parser grammar MalinaParser;

options { tokenVocab=MalinaLexer; }

//_inline - inline expression
//_stmt - declaration statement (always ends with NEWLINE).

dedent	:	DEDENT | EOF;
newline	:	NEWLINE | EOF;


module	:	namespace_declaration_stmt* (document_stmt | alias_def_stmt)*;

namespace_declaration_stmt	:	NAMESPACE_ID string_value newline;

document_stmt	:	DOCUMENT_ID ((block_inline newline) | ns_block | block | array);

alias_def_stmt	:	ALIAS_DEF_ID ((block_inline newline) | ns_block | block | array);

ns_block	:	COLON INDENT (namespace_declaration_stmt)+ (block_line_stmt | inline_stmt | hybrid_stmt)* dedent;

block	:	COLON INDENT (block_line_stmt | inline_stmt | hybrid_stmt)+ dedent;


array	:	(COLON | ARRAY_ITEM) INDENT (array_item_stmt | alias_stmt)+ dedent;

block_inline	:	COLON (inline_expression)+ COMMA?;

inline_stmt	:	inline_expression+ newline;

hybrid_stmt	:	inline_expression+ block_line_stmt;

//Represent inline expression
inline_expression	:	attr_inline | element_inline | parameter_inline | alias_inline | scope_inline;

//Represents one line of block. Always ends with NEWLINE
block_line_stmt	:	attr_stmt | element_stmt | parameter_stmt | alias_stmt | scope_stmt;

//ELEMENT RULES 

//statements
element_stmt	:	block_element_stmt | value_element_stmt | empty_element_stmt;
value_element_stmt	:	ELEMENT_ID value newline;
block_element_stmt	:	ELEMENT_ID ((block_inline newline) | block | array );
empty_element_stmt	:	ELEMENT_ID newline;

//inline
element_inline	:	empty_element_inline | value_element_inline | block_element_inline;
empty_element_inline	:	ELEMENT_ID;
value_element_inline	:	ELEMENT_ID value_inline;
block_element_inline	:	ELEMENT_ID block_inline;

//ARRAY RULES
//statements
array_item_stmt	:	block_array_item_stmt | value_array_item_stmt;
block_array_item_stmt	:	ARRAY_ITEM INDENT (block_line_stmt | inline_stmt | hybrid_stmt)+ dedent;
value_array_item_stmt	:	EQUAL value;

//SCOPE RULES

scope_stmt	:	SCOPE_ID ((block_inline newline) | block | array);
scope_inline	:	SCOPE_ID block_inline;

//ATTRIBUTE RULES

//statements
attr_stmt	:	ATTRIBUTE_ID (value)? newline;

//inline
attr_inline	:	value_attr_inline | empty_attr_inline;
value_attr_inline	: ATTRIBUTE_ID value_inline;
empty_attr_inline	: ATTRIBUTE_ID;

//PARAMETER RULES
//statements
parameter_stmt	:	empty_parameter_stmt | value_parameter_stmt | block_parameter_stmt;
empty_parameter_stmt	: PARAMETER_ID newline;
value_parameter_stmt	: PARAMETER_ID value newline;
block_parameter_stmt	: PARAMETER_ID ((block_inline newline) | block | array);

//inline
parameter_inline	:	empty_parameter_inline | value_parameter_inline | block_parameter_inline;
empty_parameter_inline	: PARAMETER_ID;
value_parameter_inline	: PARAMETER_ID value_inline;
block_parameter_inline	: PARAMETER_ID block_inline;

//ALIAS RULES
//statements
alias_stmt	:	empty_alias_stmt | value_alias_stmt | block_alias_stmt;
empty_alias_stmt	:	ALIAS_ID newline;
value_alias_stmt	:	ALIAS_ID value newline;
block_alias_stmt	:	ALIAS_ID (((block_inline | argument_block_inline) newline) | (block | array | argument_block));

//inline
alias_inline	:	empty_alias_inline | value_alias_inline | block_alias_inline;
empty_alias_inline	: ALIAS_ID;
value_alias_inline	: ALIAS_ID value_inline;
block_alias_inline	: ALIAS_ID (block_inline | argument_block_inline);

//ARGUMENTS RULES
//statements
argument_stmt	:	empty_argument_stmt | value_argument_stmt | block_argument_stmt;
empty_argument_stmt	:	ARGUMENT_ID newline;
value_argument_stmt	:	ARGUMENT_ID value newline;
block_argument_stmt	:	ARGUMENT_ID ((block_inline newline) | block | array);

argument_inline_stmt	:	argument_inline+ newline;

//inline
argument_inline	:	empty_argument_inline | value_argument_inline | block_argument_inline;
empty_argument_inline	: ARGUMENT_ID;
value_argument_inline	: ARGUMENT_ID value_inline;
block_argument_inline	: ARGUMENT_ID block_inline;

//block
argument_block	:	COLON INDENT (argument_stmt | argument_inline_stmt)+ dedent;
argument_block_inline	:	COLON (argument_inline)+ COMMA?;


//VALUES

value	:	 string_value | object_value;
value_inline	:	string_value_inline | object_value_inline;
value_ml	:	string_value_ml | object_value_ml;

//string values
string_value	:	string_value_inline | string_value_ml;
string_value_inline	:	(EQUAL | DBL_EQUAL) (OPEN_VALUE | DQS);
string_value_ml	:	(EQUAL | DBL_EQUAL) (DQS_ML | OPEN_VALUE_ML);

//object values
object_value	:	object_value_ml | object_value_inline;

object_value_ml	:	parameter_object_value_ml | alias_object_value_ml;
object_value_inline	:	parameter_object_value_inline | alias_object_value_inline;

parameter_object_value_inline	:	EQUAL PARAMETER_ID value_inline?;
parameter_object_value_ml	:	EQUAL PARAMETER_ID value_ml;

alias_object_value_inline	:	EQUAL ALIAS_ID value_inline?;
alias_object_value_ml	:	EQUAL ALIAS_ID value_ml;





