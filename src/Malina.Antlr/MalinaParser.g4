// Copyright © 2016, 2017 Maxim Trushin (dba Syntactik, trushin@gmail.com, maxim.trushin@syntactik.com)
//
// This file is part of Malina.
// Malina is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Malina is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Malina.  If not, see <http://www.gnu.org/licenses/>.

parser grammar MalinaParser;

options { tokenVocab=MalinaLexer; }

// Naming conventions:
// _inline - inline expression
// _stmt - declaration statement (always ends with newline).

dedent	:	DEDENT | EOF;
newline	:	NEWLINE | EOF;

//MODULE
module	: NEWLINE* namespace_declaration_stmt* (document_stmt | alias_def_stmt)* NEWLINE*;

namespace_declaration_stmt	:	NAMESPACE_ID string_value newline;

document_stmt	:	DOCUMENT_ID COLON ((block_inline newline) | ns_block | block);

alias_def_stmt	:	ALIAS_DEF_ID ( (value newline) | (COLON ((block_inline? newline) | ns_block | block)));


//BLOCKS
ns_block	:	INDENT (namespace_declaration_stmt)+ block_stmt* dedent;

block	:	INDENT (block_stmt+ | empty_array_item_stmt | empty_object_stmt) dedent;

//block_stmt is a statement that can be declared in the block and takes at least one line
block_stmt: 
			inline_stmt // takes exactly one line
			| block_line_stmt //takes at least two lines
			| hybrid_stmt; // start with inline expression but end with block_line_stmt

block_inline	:	(inline_expression+ COMMA?) | empty_object_inline | empty_array_inline;

//STATEMENTS and EXPRESSIONS
inline_stmt	:	inline_expression+ newline;

hybrid_stmt	:	inline_expression+ block_line_stmt;

//Represent inline expression
inline_expression	:	attr_inline | argument_inline | element_inline | parameter_inline | alias_inline | scope_inline | array_item_inline;

//Represents one line of block. Always ends with NEWLINE
block_line_stmt	:	attr_stmt | argument_stmt | alias_stmt | element_stmt | parameter_stmt | scope_stmt | array_item_stmt;

//ELEMENT RULES 
//statements
element_stmt	:	block_element_stmt | value_element_stmt | hybrid_block_element_stmt | empty_element_stmt;
value_element_stmt	:	ELEMENT_ID value newline;
block_element_stmt	:	ELEMENT_ID COLON block;
hybrid_block_element_stmt	:	ELEMENT_ID COLON (hybrid_block_element_stmt | hybrid_stmt | block_line_stmt);
empty_element_stmt	:	ELEMENT_ID (COLON COMMA?)? newline;

//inline
element_inline	:	block_element_inline | value_element_inline | empty_element_inline;
empty_element_inline	:	ELEMENT_ID  (COLON COMMA?)?;
value_element_inline	:	ELEMENT_ID value_inline;
block_element_inline	:	ELEMENT_ID COLON block_inline;


//ARRAY RULES
//statements
array_item_stmt	:	block_array_item_stmt | value_array_item_stmt | hybrid_block_array_item_stmt;
block_array_item_stmt	:	ARRAY_ITEM block;
value_array_item_stmt	:	value newline;
hybrid_block_array_item_stmt	:	ARRAY_ITEM (hybrid_block_array_item_stmt | hybrid_stmt | block_line_stmt);
empty_array_item_stmt	: (EMPTY_ARRAY | (ARRAY_ITEM COMMA?)) newline;

//inline
array_item_inline	:	value_array_item_inline | block_array_item_inline;
value_array_item_inline : value_inline;
block_array_item_inline	: ARRAY_ITEM block_inline;
empty_array_inline	:	(EMPTY_ARRAY | (ARRAY_ITEM COMMA?));	

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
empty_parameter_stmt	: PARAMETER_ID (COLON COMMA?)? newline;
value_parameter_stmt	: PARAMETER_ID value newline;
block_parameter_stmt	: PARAMETER_ID COLON block;
hybrid_block_parameter_stmt	:	PARAMETER_ID COLON (hybrid_block_parameter_stmt | hybrid_stmt | block_line_stmt);

//inline
parameter_inline	:	value_parameter_inline | block_parameter_inline | empty_parameter_inline;
empty_parameter_inline	: PARAMETER_ID (COLON COMMA?)?;
value_parameter_inline	: PARAMETER_ID value_inline;
block_parameter_inline	: PARAMETER_ID COLON block_inline;

//ALIAS RULES
//statements
alias_stmt	:	block_alias_stmt | value_alias_stmt | hybrid_block_alias_stmt | empty_alias_stmt;
empty_alias_stmt	:	ALIAS_ID (COLON COMMA?)? newline;
value_alias_stmt	:	ALIAS_ID value newline;
block_alias_stmt	:	ALIAS_ID COLON block;
hybrid_block_alias_stmt	:	ALIAS_ID COLON (hybrid_block_element_stmt | hybrid_stmt | block_line_stmt);


//inline
alias_inline	:	block_alias_inline | value_alias_inline | empty_alias_inline;
empty_alias_inline	: ALIAS_ID (COLON COMMA?)?;
value_alias_inline	: ALIAS_ID value_inline;
block_alias_inline	: ALIAS_ID COLON block_inline;

//ARGUMENTS RULES

//statements
argument_stmt	:	block_argument_stmt | hybrid_block_argument_stmt | value_argument_stmt | empty_argument_stmt;
empty_argument_stmt	:	ARGUMENT_ID (COLON COMMA?)? newline;
value_argument_stmt	:	ARGUMENT_ID value newline;
block_argument_stmt	:	ARGUMENT_ID COLON block;
hybrid_block_argument_stmt	:	ARGUMENT_ID COLON ( hybrid_stmt | block_line_stmt);


//inline
argument_inline	:	value_argument_inline | block_argument_inline | empty_argument_inline;
empty_argument_inline	: ARGUMENT_ID (COLON COMMA?)?;
value_argument_inline	: ARGUMENT_ID value_inline;
block_argument_inline	: ARGUMENT_ID  COLON block_inline;


//VALUES
value	:	 string_value | object_value;
value_inline	:	string_value_inline | object_value_inline;
value_ml	:	string_value_ml | object_value_ml;

//string values
string_value	:	string_value_inline | string_value_ml;
string_value_inline	:	(EQUAL | DBL_EQUAL) (OPEN_STRING | FREE_OPEN_STRING | JSON_BOOLEAN | JSON_NULL | JSON_NUMBER | DQS | sqs_json_literal | sqs_inline);
string_value_ml	:	(EQUAL | DBL_EQUAL) (sqs_ml | DQS_ML | OPEN_STRING_ML | FREE_OPEN_STRING_ML);


sqs_inline	:	SQS sqs_body_item* SQS_END?;
sqs_ml	:	SQS sqs_body_item* SQS_EOL (sqs_body_item | SQS_EOL)* SQS_END?;

sqs_body_item	:	SQS_VALUE | INTERPOLATION | SQS_JSON_BOOLEAN | SQS_JSON_NULL | SQS_JSON_NUMBER | SQS_ESCAPE;

sqs_json_literal	:	SQS	(SQS_JSON_BOOLEAN | SQS_JSON_NULL | SQS_JSON_NUMBER) SQS_END?;
//object values
object_value	:	object_value_ml | object_value_inline;

object_value_ml	:	parameter_object_value_ml | alias_object_value_ml;
object_value_inline	:	parameter_object_value_inline | alias_object_value_inline;

parameter_object_value_inline	:	EQUAL parameter_inline;
parameter_object_value_ml	:	EQUAL parameter_stmt;

alias_object_value_inline	:	EQUAL alias_inline;
alias_object_value_ml	:	EQUAL alias_stmt;

//Empty object 
empty_object_inline		:	EMPTY_OBJECT;
empty_object_stmt		:	EMPTY_OBJECT newline;
