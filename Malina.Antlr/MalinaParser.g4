parser grammar MalinaParser;

options { tokenVocab=MalinaLexer; }

//_inline - inline expression
//_stmt - declaration statement (always ends with NEWLINE).

module			:	namespace_decl_stmt* (document_stmt | alias_def_stmt)*;

namespace_decl_stmt : NAMESPACE_ID;

document_stmt	:	DOCUMENT_ID ((block_inline NEWLINE) | block);

alias_def_stmt	:	ALIAS_DEF_ID ((block_inline NEWLINE) | block);

block	:	COLON INDENT (block_line_stmt | inline_stmt | hybrid_stmt)+ DEDENT;

block_inline	:	COLON (inline_expression)+ COMMA?;

inline_stmt	:	inline_expression+ NEWLINE;

hybrid_stmt	:	inline_expression+ block_line_stmt;

//Represent inline expression
inline_expression	:	attr_inline | element_inline | parameter_inline | alias_inline;

//Represents one line of block. Always ends with NEWLINE
block_line_stmt	:	attr_stmt | element_stmt | parameter_stmt | alias_stmt;

//ELEMENT RULES 

//statements
element_stmt	:	block_element_stmt | value_element_stmt | empty_element_stmt;
value_element_stmt	:	ELEMENT_ID value NEWLINE;
block_element_stmt	:	ELEMENT_ID ((block_inline NEWLINE) | block);
empty_element_stmt	:	ELEMENT_ID NEWLINE;

//inline
element_inline	:	empty_element_inline | value_element_inline | block_element_inline;
empty_element_inline	:	ELEMENT_ID;
value_element_inline	:	ELEMENT_ID value_inline;
block_element_inline	:	ELEMENT_ID block_inline;

//ATTRIBUTE RULES

//statements
attr_stmt	:	ATTRIBUTE_ID (value | parameter_inline | alias_inline)? NEWLINE;

//inline
attr_inline	:	value_attr_inline | empty_attr_inline;
value_attr_inline	: ATTRIBUTE_ID value_inline;
empty_attr_inline	: ATTRIBUTE_ID;

//PARAMETER RULES
//statements
parameter_stmt	:	empty_parameter_stmt | value_parameter_stmt | block_parameter_stmt;
empty_parameter_stmt	: PARAMETER_ID NEWLINE;
value_parameter_stmt	: PARAMETER_ID value NEWLINE;
block_parameter_stmt	: PARAMETER_ID ((block_inline NEWLINE) | block);

//inline
parameter_inline	:	empty_parameter_inline | value_parameter_inline | block_parameter_inline;
empty_parameter_inline	: PARAMETER_ID;
value_parameter_inline	: PARAMETER_ID value_inline;
block_parameter_inline	: PARAMETER_ID block_inline;

//ALIAS RULES
//statements
alias_stmt	:	empty_alias_stmt | value_alias_stmt | block_alias_stmt;
empty_alias_stmt	:	ALIAS_ID NEWLINE;
value_alias_stmt	:	ALIAS_ID value NEWLINE;
block_alias_stmt	:	ALIAS_ID ((block_inline NEWLINE) | block);

//inline
alias_inline	:	empty_alias_inline | value_alias_inline | block_alias_inline;
empty_alias_inline	: ALIAS_ID;
value_alias_inline	: ALIAS_ID value_inline;
block_alias_inline	: ALIAS_ID block_inline;

value	:	 string_value | object_value;
value_inline	:	string_value_inline | object_value_inline;
value_ml	:	string_value_ml | object_value_ml;

string_value	:	string_value_inline | string_value_ml;
string_value_inline	:	(EQUAL | DBL_EQUAL) (OPEN_VALUE | DQS);
string_value_ml	:	(EQUAL | DBL_EQUAL) (DQS_ML | open_value_ml);

open_value_ml	:	(OPEN_VALUE | OPEN_VALUE_INDENT)+;

object_value	:	object_value_ml | object_value_inline;

object_value_ml	:	parameter_object_value_ml | alias_object_value_ml;
object_value_inline	:	parameter_object_value_inline | alias_object_value_inline;

parameter_object_value_inline	:	PARAMETER_ID value_inline;
parameter_object_value_ml	:	PARAMETER_ID value_ml;

alias_object_value_inline	:	ALIAS_ID value_inline;
alias_object_value_ml	:	ALIAS_ID value_ml;





