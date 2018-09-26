CREATE OR REPLACE PACKAGE BODY PHAP_OWNER.PAT2 AS

PROCEDURE find_patterns (sql_stmt IN VARCHAR2, sql_stmt2 IN VARCHAR2, collec1 OUT clob, collec2 OUT clob) IS-- collec OUT base4) IS   
  
    src_cur         curtype;
    curid           NUMBER;
    desctab         DBMS_SQL.DESC_TAB;
    itemcount       NUMBER;
    namevar         VARCHAR2(200);
  
    src_cur2        curtype;
    curid2          NUMBER;
    desctab2        DBMS_SQL.DESC_TAB;
    itemcount2      NUMBER;
    namevar2        VARCHAR2(200);
    
    colcount        NUMBER;
    matchcounter    NUMBER;
    
    ro number;

BEGIN

    -- create dynaic cursor for first and second query
    OPEN src_cur FOR sql_stmt;
    OPEN src_cur2 FOR sql_stmt2;

    -- switch from native dynamic SQL to DBMS_SQL package:
    switch_to_dbms_package(src_cur,desctab,curid,itemcount);
    switch_to_dbms_package(src_cur2,desctab2,curid2,itemcount2);
    
    -- make itemcount global 
    itemcount3 := itemcount;
    
    -- define columns for first and second dynamic cursor: 
    define_columns(curid,namevar,50,itemcount);
    define_columns(curid2,namevar2,50,itemcount2);
    
    -- populate subject and result set collection
    populate_subject_collection(desctab,curid,namevar,itemcount);

--    populate_result_set_collection (curid2,desctab,colcount,namevar2);

    colcount := desctab.count;
  
    -- count the number of null values in subject record -- this is needed to
    -- get the count of records with exact matches. Null values aren't counted
    -- so they need to be subtracted
    FOR b IN 1 .. colcount LOOP
        IF new_pat(2)(b) = ' ' OR new_pat(2)(b) IS NULL THEN
          noValueCount := noValueCount + 1;
        END IF;
    END LOOP;
      
    -- check for matches - compare subject record and result set records
--    FOR i IN 1 .. new_pat2.count LOOP
    ro := 1;
    WHILE DBMS_SQL.FETCH_ROWS(curid2) > 0 LOOP  
    matchcounter := 0; 
        FOR b IN 1 .. itemcount LOOP             
        DBMS_SQL.COLUMN_VALUE(curid2, b, namevar2);                      
            IF new_pat(2)(b) = namevar2 THEN
                new_pat2(ro)(b) := namevar2||'|1 ';
                new_pat(3)(b) := new_pat(3)(b) + 1;
                matchcounter := matchcounter + 1;
            ELSE
                new_pat2(ro)(b) := namevar2||'|0 ';
            END IF;
        
        END LOOP;                
        new_pat2(ro)(colcount + 1) := matchcounter;
        ro := ro + 1;
        IF matchcounter > 0 THEN    
            recsWMatches := recsWMatches + 1;  
        END IF;
        IF matchcounter = (colcount - noValueCount) THEN
          recsWExactMatches := recsWExactMatches + 1;
        END IF;          
    END LOOP;
--    
--    -- sort records by from highest match to lowest match count (insertion sort)    
    insertion_sort(itemcount,colcount);
    columnCount :=  colcount;
--     quicksort(1,new_pat2.count); -- work in progress   
--     
    totalRows := new_pat2.count; 
    
         
    -- create the subject record xml  
    collec1 := get_subject_xml(itemcount);
           
    -- create result set xml
    collec2 := get_resultset_xml(1);
 
END;

PROCEDURE switch_to_dbms_package (p_cur IN OUT curtype, p_desctab IN OUT DBMS_SQL.DESC_TAB, p_cur_id OUT NUMBER, icount IN OUT NUMBER)
IS
BEGIN
    -- gets a unique id for dynamic cursor
    p_cur_id := DBMS_SQL.TO_CURSOR_NUMBER(p_cur);
    --gets number of columns and name of columsn
    DBMS_SQL.DESCRIBE_COLUMNS(p_cur_id, icount, p_desctab);
END;

PROCEDURE define_columns (p_cur IN OUT INTEGER,p_nvar IN VARCHAR, p_length IN INTEGER,p_itemcount IN NUMBER)
IS
BEGIN
    FOR i IN 1 .. p_itemcount LOOP
        DBMS_SQL.DEFINE_COLUMN(p_cur, i, p_nvar, p_length);
    END LOOP;
END;

PROCEDURE populate_subject_collection(p_desctab IN OUT DBMS_SQL.DESC_TAB, p_curid IN OUT NUMBER, p_namevar IN OUT VARCHAR2, p_itemcount IN NUMBER)
IS
BEGIN
 
-- assign the column names to the subject collection
  FOR i IN 1 .. p_desctab.count LOOP
        new_pat(1)(i) := p_desctab(i).col_name;
    END LOOP;

    -- assign column values to the subject collection
    WHILE DBMS_SQL.FETCH_ROWS(p_curid) > 0 LOOP  
        FOR i IN 1 .. p_itemcount LOOP
       
            DBMS_SQL.COLUMN_VALUE(p_curid, i, p_namevar);
            new_pat(2)(i) := to_char(p_namevar);
    
        END LOOP;
    END LOOP;
  
    -- initialize the match count for each column name to 0
    FOR i IN 1 .. p_desctab.count LOOP
        new_pat(3)(i) := 0;
    END LOOP;

END;

PROCEDURE populate_result_set_collection (p_curid IN OUT NUMBER,p_desctab IN OUT DBMS_SQL.DESC_TAB, p_colcount IN NUMBER, p_namevar IN OUT VARCHAR2)
IS
    v_colcount    NUMBER := p_desctab.count;
    v_nextrow     NUMBER:= v_colcount;
    v_tonextrow   NUMBER:= v_colcount;
    v_rowposition NUMBER:= 1;
    v_tcount      NUMBER:= 1;
    
BEGIN

    WHILE DBMS_SQL.FETCH_ROWS(p_curid) > 0 LOOP  
        FOR i IN 1 .. v_colcount LOOP            
            
            DBMS_SQL.COLUMN_VALUE(p_curid, i, p_namevar);
            new_pat2(v_rowposition)(i) := to_char(p_namevar);     
            IF v_tcount = v_nextrow THEN
                v_rowposition := v_rowposition + 1;                       
                v_nextrow := v_nextrow + v_tonextrow; 
            END IF;      
            v_tcount := v_tcount + 1;
        
        END LOOP;
    END LOOP;
    
END;
 
FUNCTION get_subject_xml (p_itemcount IN NUMBER)
    RETURN CLOB
IS
    xmldata CLOB;
BEGIN
    
    xmldata := xmldata ||'<?xml version = "1.0"?>';
    xmldata := xmldata ||'<rows xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">'; 
    
    --create row
    FOR i IN 1 .. new_pat.count LOOP
        xmldata := xmldata ||'<row>';
       
        --create column 
        FOR b IN 1 .. p_itemcount LOOP 
            xmldata := xmldata ||'<column>'||new_pat(i)(b)||'</column>';
        END LOOP;
    
        xmldata := xmldata ||'</row>';
        
    END LOOP; 
  
    xmldata := xmldata ||'</rows>';
    
    RETURN xmldata;
    
END;
 
FUNCTION get_resultset_xml (rowpos IN INTEGER)
    RETURN CLOB
IS
   xmldata CLOB;
   endOfLoop INTEGER;
BEGIN

    IF totalRows < rowpos + 49 THEN
        endOfLoop := new_pat2.count;
    ELSE
        endOFLoop := rowpos + 49;
    END IF;
    
    --create header rows columns for the result set 
    xmldata := xmldata ||'<?xml version = "1.0"?>';
    xmldata := xmldata ||'<rows xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">';  
    xmldata := xmldata ||'<row>';
  
    --create headers for rows
    FOR b IN 1 .. itemcount3 LOOP 
        xmldata := xmldata ||'<column>'||new_pat(1)(b)||'</column>';
    END LOOP; 
  
    xmldata := xmldata ||'</row>';       
 
    --create rows
    FOR i IN rowpos .. endOfLoop LOOP
        xmldata := xmldata ||'<row totalMatches="'||new_pat2(i)(itemcount3 + 1)||'">';

        --create columns
        FOR b IN 1 .. itemcount3 LOOP 
            xmldata := xmldata ||'<column match="'||trim(substr(new_pat2(i)(b),(instr(new_pat2(i)(b),'|',1,1)+ 1)))||'">'||substr(new_pat2(i)(b),1,(instr(new_pat2(i)(b),'|',1,1) - 1))||'</column>';
        END LOOP;
    
        xmldata := xmldata ||'</row>';
    END LOOP; 
    xmldata := xmldata|| '<stats totalRows="'||to_char(totalRows)||'" columnCount="'||to_char(columnCount)||'" recsWMatches="'||to_char(recsWMatches)||'" recsWExactMatches="'||to_char(recsWExactMatches)||'"></stats>';

    xmldata := xmldata ||'</rows>';
    
    RETURN xmldata; 
    
END;

PROCEDURE insertion_sort(p_itemcount IN NUMBER, p_colcount NUMBER)
IS
    curr INTEGER;
    prevv INTEGER;
BEGIN
FOR i in 2 .. new_pat2.count LOOP  
        curr := i;
        prevv := i - 1;   
        
        WHILE to_number(new_pat2(prevv)(p_itemcount + 1)) < to_number(new_pat2(curr)(p_itemcount + 1)) LOOP

            --swap the matchcounter fields
            temp_pat(1)(p_itemcount + 1) := new_pat2(curr)(p_itemcount + 1);
            new_pat2(curr)(p_itemcount + 1) := new_pat2(prevv)(p_itemcount + 1);
            new_pat2(prevv)(p_itemcount + 1) := temp_pat(1)(p_itemcount + 1);           
         
            --swap records
            FOR b IN 1 .. p_colcount LOOP
                temp_pat(1)(b) := new_pat2(curr)(b);            
                new_pat2(curr)(b) := new_pat2(prevv)(b);
                new_pat2(prevv)(b) := temp_pat(1)(b);        
            END LOOP;           
            curr := curr - 1;
            prevv := prevv - 1;                
            IF curr = 1 THEN
            EXIT;
            END IF;       
        
        END LOOP;   
    END LOOP;
END;

PROCEDURE quicksort (m IN NUMBER, n IN NUMBER ) 
IS
    --pivot NUMBER;--:= first;
    i NUMBER;-- := first;
    j NUMBER;-- := last;
    k NUMBER;
    
--    temp INTEGER;
    kee NUMBER;
BEGIN

    IF m < n THEN   
        k := choose_pivot(m,n);
        swap(m,k);
        kee := TO_NUMBER(new_pat2(m)(itemcount3 + 1));
        i := m + 1;
        j := n;
        while i <= j LOOP
    
            WHILE i <= n and TO_NUMBER(new_pat2(i)(itemcount3 + 1)) <= kee LOOP
            i := i + 1;
            END LOOP;
        
            WHILE j >= m and TO_NUMBER(new_pat2(j)(itemcount3 + 1)) > kee LOOP
            j := j - 1;
            END LOOP;
        
            IF i < j THEN
                swap(i,j);
            END IF;
       
            quicksort(m, j - 1);
            quicksort(j + 1, n);
    
        END LOOP;
    END IF;
    
--       --pivot := new_pat2((first + last) / 2)(itemcount3 + 1);
--       while i < j loop 
--             while to_number(new_pat2(i)(itemcount3 + 1)) <= to_number(new_pat2(pivot)(itemcount3 + 1)) loop 
--                  if i < last then  
--                    i := i + 1; 
--                  else   
--                    EXIT WHEN i >= last; 
--                  end if;  
--             end loop; 
--             while to_number(new_pat2(j)(itemcount3 + 1)) > to_number(new_pat2(pivot)(itemcount3 + 1)) loop 
--                  if j > first then  
--                    j := j - 1; 
--                  else   
--                     EXIT WHEN j <= first; 
--                  end if;  
--             end loop; 
--             if i < j then 
--                swap(i,j);
--             elsif i >= j then 
--                swap(pivot,j);
--             end if; 
--          end loop; 
--          if j - 1 > 1 then 
--             quicksort(1, j - 1); 
--          end if; 
--          if  j + 1 < last then 
--             quicksort(j + 1,last);  
--          end if; 
    
END quicksort;
    
PROCEDURE swap (x IN NUMBER, y NUMBER) 
IS     
BEGIN
    temp_pat(1)(itemcount3 + 1) := new_pat2(x)(itemcount3 + 1);
    new_pat2(x)(itemcount3 + 1) := new_pat2(y)(itemcount3 + 1);
    new_pat2(y)(itemcount3 + 1) := temp_pat(1)(itemcount3 + 1);               
        
    FOR b IN 1 .. columnCount LOOP          
        temp_pat(1)(b) := new_pat2(x)(b);            
        new_pat2(x)(b) := new_pat2(x)(b);
        new_pat2(y)(b) := temp_pat(1)(b); 
    END LOOP;        
       
END swap;
    
FUNCTION choose_pivot(i IN NUMBER, j IN NUMBER)
    RETURN NUMBER
IS
BEGIN
    
    RETURN FLOOR((i + j) / 2);
        
END;
 
FUNCTION test_function (p_bpid_type IN VARCHAR2)
    RETURN SYS_REFCURSOR
IS
      v_return_ref_cur   SYS_REFCURSOR;
BEGIN
    OPEN v_return_ref_cur FOR
        SELECT * from ph_raeob_chk_hdr where bpid_type = p_bpid_type and rownum < 50;      
      
    RETURN v_return_ref_cur;
END;

     
PROCEDURE test_proc (sql_stmt IN VARCHAR2, sql_stmt2 IN VARCHAR2, collec OUT varchar2) 
IS 
BEGIN
   IF sql_stmt != ' ' and sql_stmt is not null and sql_stmt2 != ' ' and sql_stmt2 is not null THEN
   
   collec := 'true';
   else
   collec :='false';

   END IF;
END; 
         
PROCEDURE test_array_proc (sql_stmt IN VARCHAR2, sql_stmt2 IN VARCHAR2, collec OUT base4) 
IS 
    pat_8 base4;
BEGIN
   
   IF sql_stmt != ' ' and sql_stmt is not null and sql_stmt2 != ' ' and sql_stmt2 is not null THEN
   
    pat_8(1)(1) := 'Return List';
   
    collec := pat_8;

   END IF;
   
END; 
   
FUNCTION test_ref (q1 IN VARCHAR2,q2 IN VARCHAR2)
    RETURN SYS_REFCURSOR
IS
    v_return_ref_cur   SYS_REFCURSOR;
    new_pat3 clob;
    new_pat4 clob;
BEGIN
   
    find_patterns(q1, q2, new_pat3, new_pat4);
   
--      OPEN v_return_ref_cur FOR
--         SELECT * from table(new_pat3);   
      
      RETURN v_return_ref_cur;
END;
   
PROCEDURE test_xml(xdata out clob) 
IS
    xmldata clob;
BEGIN
   
    xmldata := xmldata ||'<?xml version = "1.0"?>';
    xmldata := xmldata ||'<rows xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">';
    xmldata := xmldata ||'<row totalMatches="3">';
    xmldata := xmldata ||'<column match="0">7569</column>';
    xmldata := xmldata ||'<column match="1">PRB</column>';
    xmldata := xmldata ||'<column match="1">AER</column>';
    xmldata := xmldata ||'<column match="1">NOT</column>';
    xmldata := xmldata ||'</row>';
    xmldata := xmldata ||'<row totalMatches="1">';
    xmldata := xmldata ||'<column match="0">7568</column>';
    xmldata := xmldata ||'<column match="0">PRA</column>';
    xmldata := xmldata ||'<column match="0">AEB</column>';
    xmldata := xmldata ||'<column match="1">NOT</column>';
    xmldata := xmldata ||'</row>';
    xmldata := xmldata ||'<row totalMatches="2">';
    xmldata := xmldata ||'<column match="1">7542</column>';
    xmldata := xmldata ||'<column match="1">PRB</column>';
    xmldata := xmldata ||'<column match="0">AEW</column>';
    xmldata := xmldata ||'<column match="0">NOD</column>';
    xmldata := xmldata ||'</row>';
    xmldata := xmldata ||'<row totalMatches="1">';
    xmldata := xmldata ||'<column match="0">7125</column>';
    xmldata := xmldata ||'<column match="0">PRS</column>';
    xmldata := xmldata ||'<column match="0">AET</column>';
    xmldata := xmldata ||'<column match="1">NOT</column>';
    xmldata := xmldata ||'</row>';
    xmldata := xmldata ||'</rows>';

    xdata := xmldata;
    
    END;
  
END PAT2;
/
