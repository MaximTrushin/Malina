parser grammar MalinaParser;

options { tokenVocab=MalinaLexer; }

// Naming conventions:
// _inline - inline expression
// _stmt - declaration statement (always ends with NEWLINE or EOF).

dedent	:	DEDENT | EOF;
newline	:	NEWLINE | EOF;

//MODULE
module	: NEWLINE* namespace_declaration_stmt* (document_stmt | alias_def_stmt)* NEWLINE*;

namespace_declaration_stmt	:	NAMESPACE_ID string_value newline;

document_stmt	:	DOCUMENT_ID COLON ((block_inline newline) | ns_block | block /*| array*/);

alias_def_stmt	:	ALIAS_DEF_ID ( (value newline) | (COLON ((block_inline newline) | ns_block | block | array)));


//BLOCKS
ns_block	:	INDENT (namespace_declaration_stmt)+ block_stmt* dedent;

block	:	INDENT (block_stmt | array_stmt)+ dedent;

array	: INDENT array_stmt + dedent;

//block_stmt is a statement that can be declared in the block and takes at least one line
block_stmt: 
			inline_stmt // takes exactly one line
			| block_line_stmt //takes at least two lines
			| hybrid_stmt; // start with inline expression but end with block_line_stmt

array_stmt: 
			array_inline_stmt // takes exactly one line
			| array_line_stmt //takes at least two lines
			| array_hybrid_stmt; // start with inline expression but end with block_line_stmt

block_inline	:	inline_expression+ COMMA?;

//STATEMENTS and EXPRESSIONS
inline_stmt	:	inline_expression+ newline;
array_inline_stmt : inline_array_expression* array_item_inline inline_array_expression* newline;

hybrid_stmt	:	inline_expression+ block_line_stmt;
array_hybrid_stmt : inline_array_expression+ array_line_stmt;

//Represent inline expression
inline_expression	:	attr_inline | element_inline | parameter_inline | alias_inline | scope_inline;
inline_array_expression: array_item_inline | parameter_inline | alias_inline;

//Represents one line of block. Always ends with NEWLINE
block_line_stmt	:	attr_stmt | alias_stmt | element_stmt | parameter_stmt | scope_stmt;
array_line_stmt: parameter_stmt | alias_stmt | array_item_stmt;


//ELEMENT RULES 
//statements
element_stmt	:	block_element_stmt | value_element_stmt | hybrid_block_element_stmt | empty_element_stmt;
value_element_stmt	:	ELEMENT_ID value newline;
block_element_stmt	:	ELEMENT_ID COLON (block /*| array*/);
hybrid_block_element_stmt	:	ELEMENT_ID COLON (hybrid_block_element_stmt | hybrid_stmt | block_line_stmt);
empty_element_stmt	:	ELEMENT_ID COLON? newline;

//inline
element_inline	:	value_element_inline | block_element_inline | empty_element_inline;
empty_element_inline	:	ELEMENT_ID;
value_element_inline	:	ELEMENT_ID value_inline;
block_element_inline	:	ELEMENT_ID COLON block_inline;

//ARRAY RULES
//statements
array_item_stmt	:	block_array_item_stmt | value_array_item_stmt | hybrid_block_array_item_stmt;
block_array_item_stmt	:	ARRAY_ITEM (block | array);
value_array_item_stmt	:	EQUAL value newline;
hybrid_block_array_item_stmt	:	ARRAY_ITEM (hybrid_block_array_item_stmt | hybrid_stmt | block_line_stmt);

//inline
array_item_inline	:	value_array_item_inline | block_array_item_inline;
value_array_item_inline : value_inline;
block_array_item_inline	: ARRAY_ITEM block_inline;

//SCOPE RULES
scope_stmt	:	SCOPE_ID COLON block;
scope_inline	:	SCOPE_ID COLON block_inline;

//ATTRIBUTE RULES

//statements
attr_stmt	:	ATTRIBUTE_ID (value)? newline;

//inline
attr_inline	:	value_attr_inline | empty_attr_inline;
value_attr_inline	: ATTRIBUTE_ID value_inline;
empty_attr_inline	: ATTRIBUTE_ID;

//PARAMETER RULES
//statements
parameter_stmt	:	value_parameter_stmt | block_parameter_stmt | hybrid_block_parameter_stmt | empty_parameter_stmt;
empty_parameter_stmt	: PARAMETER_ID newline;
value_parameter_stmt	: PARAMETER_ID value newline;
block_parameter_stmt	: PARAMETER_ID COLON (block | array);
hybrid_block_parameter_stmt	:	PARAMETER_ID COLON (hybrid_block_parameter_stmt | hybrid_stmt | block_line_stmt);

//inline
parameter_inline	:	value_parameter_inline | block_parameter_inline | empty_parameter_inline;
empty_parameter_inline	: PARAMETER_ID;
value_parameter_inline	: PARAMETER_ID value_inline;
block_parameter_inline	: PARAMETER_ID COLON block_inline;

//ALIAS RULES
//statements
alias_stmt	:	value_alias_stmt | block_alias_stmt | hybrid_block_alias_stmt | empty_alias_stmt;
empty_alias_stmt	:	ALIAS_ID newline;
value_alias_stmt	:	ALIAS_ID value newline;
block_alias_stmt	:	ALIAS_ID COLON (block | array | argument_block);
hybrid_block_alias_stmt	:	ALIAS_ID COLON (hybrid_block_alias_stmt | hybrid_stmt | block_line_stmt);


//inline
alias_inline	:	value_alias_inline | block_alias_inline | empty_alias_inline;
empty_alias_inline	: ALIAS_ID;
value_alias_inline	: ALIAS_ID value_inline;
block_alias_inline	: ALIAS_ID COLON (block_inline | argument_block_inline);

//ARGUMENTS RULES
//block
argument_block	:	INDENT (argument_inline_stmt | argument_stmt)+ dedent;
argument_block_inline	:	(argument_inline)+ COMMA?;

//statements
argument_stmt	:	value_argument_stmt | block_argument_stmt | hybrid_block_argument_stmt | empty_argument_stmt;
empty_argument_stmt	:	ARGUMENT_ID newline;
value_argument_stmt	:	ARGUMENT_ID value newline;
block_argument_stmt	:	ARGUMENT_ID COLON (block | array);
hybrid_block_argument_stmt	:	ARGUMENT_ID COLON (hybrid_block_argument_stmt | hybrid_stmt | block_line_stmt);

argument_inline_stmt	:	argument_inline+ newline;

//inline
argument_inline	:	value_argument_inline | block_argument_inline | empty_argument_inline;
empty_argument_inline	: ARGUMENT_ID;
value_argument_inline	: ARGUMENT_ID value_inline;
block_argument_inline	: ARGUMENT_ID  COLON block_inline;


//VALUES

value	:	 string_value | object_value;
value_inline	:	string_value_inline | object_value_inline;
value_ml	:	string_value_ml | object_value_ml;

//string values
string_value	:	string_value_inline | string_value_ml;
string_value_inline	:	(EQUAL | DBL_EQUAL) (OPEN_VALUE | DQS | sqs_inline);
string_value_ml	:	(EQUAL | DBL_EQUAL) (sqs_ml | DQS_ML | OPEN_VALUE_ML);

sqs_inline	:	SQS (SQS_VALUE | INTERPOLATION)* SQS_END?;
sqs_ml	:	SQS (SQS_VALUE | INTERPOLATION)* SQS_EOL (SQS_VALUE | INTERPOLATION | SQS_EOL)* SQS_END?;

//object values
object_value	:	object_value_ml | object_value_inline;

object_value_ml	:	parameter_object_value_ml | alias_object_value_ml;
object_value_inline	:	parameter_object_value_inline | alias_object_value_inline;

parameter_object_value_inline	:	EQUAL PARAMETER_ID value_inline?;
parameter_object_value_ml	:	EQUAL PARAMETER_ID value_ml;

alias_object_value_inline	:	EQUAL ALIAS_ID value_inline?;
alias_object_value_ml	:	EQUAL ALIAS_ID value_ml;

