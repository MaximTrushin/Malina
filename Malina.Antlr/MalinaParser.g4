parser grammar MalinaParser;

options { tokenVocab=MalinaLexer; }

//_inline - inline expression
//_stmt - declaration statement (always ends with NEWLINE).

module			:	namespace_decl_stmt* (document_stmt | alias_def_stmt)*;

namespace_decl_stmt : NAMESPACE_ID;

document_stmt	:	DOCUMENT_ID ((block_inline NEWLINE) | block);

alias_def_stmt	:	ALIAS_DEF_ID ((block_inline NEWLINE) | block);

block	:	COLON INDENT (block_line_stmt | inline_stmt)+ DEDENT;

block_inline	:	COLON (inline_expression)+ COMMA?;

inline_stmt	:	inline_expression+ NEWLINE;

//Represent inline expression
inline_expression	:	attr_inline | element_inline | parameter_inline | alias_inline;

//Represents one line of block. Always ends with NEWLINE
block_line_stmt	:	attr_stmt | element_stmt | parameter_stmt | alias_stmt;



//ELEMENT RULES 

//statements
element_stmt	:	block_element_stmt | value_element_stmt | empty_element_stmt;
value_element_stmt	:	ELEMENT_ID (DQS | open_value) NEWLINE;
block_element_stmt	:	ELEMENT_ID ((block_inline NEWLINE) | block);
empty_element_stmt	:	ELEMENT_ID NEWLINE;

//inline
element_inline	:	empty_element_inline | value_element_inline | block_element_inline;
empty_element_inline	:	ELEMENT_ID;
value_element_inline	:	ELEMENT_ID (DQS | open_value);
block_element_inline	:	ELEMENT_ID block_inline;

//ATTRIBUTE RULES

//statements
attr_stmt	:	ATTRIBUTE_ID (DQS | open_value)? NEWLINE;

//inline
attr_inline	:	value_attr_inline | empty_attr_inline;
value_attr_inline	: ATTRIBUTE_ID (DQS | open_value);
empty_attr_inline	: ATTRIBUTE_ID;

//PARAMETER RULES
//statements
parameter_stmt	:	empty_parameter_stmt | value_parameter_stmt | block_parameter_stmt;
empty_parameter_stmt	: PARAMETER_ID NEWLINE;
value_parameter_stmt	: PARAMETER_ID (DQS | open_value) NEWLINE;
block_parameter_stmt	: PARAMETER_ID ((block_inline NEWLINE) | block);

//inline
parameter_inline	:	empty_parameter_inline | value_parameter_inline | block_parameter_inline;
empty_parameter_inline	: PARAMETER_ID;
value_parameter_inline	: PARAMETER_ID (DQS | open_value);
block_parameter_inline	: PARAMETER_ID block_inline;


//ALIAS RULES
//statements
alias_stmt	:	empty_alias_stmt | value_alias_stmt | block_alias_stmt;
empty_alias_stmt	:	ALIAS_ID NEWLINE;
value_alias_stmt	:	ALIAS_ID (DQS | open_value) NEWLINE;
block_alias_stmt	:	ALIAS_ID ((block_inline NEWLINE) | block);

//inline
alias_inline	:	empty_alias_inline | value_alias_inline | block_alias_inline;
empty_alias_inline	: ALIAS_ID;
value_alias_inline	: ALIAS_ID (DQS | open_value);
block_alias_inline	: ALIAS_ID block_inline;


open_value	: (OPEN_VALUE | OPEN_VALUE_INDENT)+;







