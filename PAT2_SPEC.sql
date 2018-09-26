CREATE OR REPLACE PACKAGE PHAP_OWNER.PAT2 AS    

  TYPE base IS TABLE OF VARCHAR2 (500)
  INDEX BY BINARY_INTEGER;
  
  TYPE base2 IS TABLE OF base
  INDEX BY BINARY_INTEGER;
  
  TYPE base3 IS TABLE OF base
  INDEX BY BINARY_INTEGER;
  
  TYPE base4 IS TABLE OF base
  INDEX BY BINARY_INTEGER;
  
  --  declare first dynamic cursor
  TYPE curtype IS REF CURSOR;
  -- declare second dynamic cursor
  TYPE curtype2 IS REF CURSOR;
  
  new_pat base2;
  new_pat2 base3;
  temp_pat base4;
  
  itemcount3 integer;
--  columncount INTEGER;
    
  totalRows NUMBER; 
  columnCount NUMBER;
  recsWMatches NUMBER := 0;
  recsWExactMatches NUMBER :=0;
  noValueCount NUMBER := 0;
    

  
    PROCEDURE find_patterns (sql_stmt IN VARCHAR2, sql_stmt2 IN VARCHAR2, collec1 OUT clob, collec2 OUT clob);--, stats OUT VARCHAR2);

    PROCEDURE define_columns (p_cur IN OUT INTEGER, p_nvar IN VARCHAR, p_length IN INTEGER, p_itemcount IN NUMBER);
     
    PROCEDURE switch_to_dbms_package (p_cur IN OUT curtype, p_desctab IN OUT DBMS_SQL.DESC_TAB ,p_cur_id OUT NUMBER,  icount IN OUT NUMBER);
    
    PROCEDURE populate_subject_collection(p_desctab IN OUT DBMS_SQL.DESC_TAB,p_curid IN OUT NUMBER, p_namevar IN OUT VARCHAR2, p_itemcount IN NUMBER);
    
    PROCEDURE populate_result_set_collection (p_curid IN OUT NUMBER,p_desctab IN OUT DBMS_SQL.DESC_TAB, p_colcount IN NUMBER, p_namevar IN OUT VARCHAR2);
    
    PROCEDURE insertion_sort (p_itemcount IN NUMBER, p_colcount IN NUMBER);
    
    FUNCTION get_subject_xml (p_itemcount IN NUMBER)
        RETURN CLOB;
    
    FUNCTION get_resultset_xml (rowpos IN INTEGER)
        RETURN CLOB;
        
    PROCEDURE quicksort (m IN NUMBER, n IN NUMBER);

    PROCEDURE swap (x IN NUMBER, y NUMBER);
    
    FUNCTION test_function (p_bpid_type IN VARCHAR2)
        RETURN SYS_REFCURSOR;
	  
    PROCEDURE test_proc (sql_stmt IN VARCHAR2, sql_stmt2 IN VARCHAR2, collec OUT varchar2);
	  
    PROCEDURE test_array_proc (sql_stmt IN VARCHAR2, sql_stmt2 IN VARCHAR2, collec OUT base4);
	  
    FUNCTION test_ref (q1 IN VARCHAR2,q2 IN VARCHAR2)
        RETURN SYS_REFCURSOR;

    FUNCTION choose_pivot(i IN NUMBER, j IN NUMBER)
    RETURN NUMBER;
    
--    FUNCTION do_query_3 (sql_stmt IN VARCHAR2, sql_stmt2 IN VARCHAR2)   
--        RETURN SYS_REFCURSOR;
        
END PAT2;
/
