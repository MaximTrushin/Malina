parser grammar MalinaParser;

options { tokenVocab=MalinaLexer; }

//_inline - inline expression
//_stmt - declaration statement (always ends with NEWLINE).

module			:	namespace_decl_stmt* (document_stmt | alias_def_stmt)*
				;
namespace_decl_stmt : NAMESPACE_ID;

document_stmt	:	DOCUMENT_ID COLON;// document_body;

//document_body	: document_body_inline | document_body_block;

//document_body_inline	:	

alias_def_stmt	:	ALIAS_DEF_ID ((block_inline NEWLINE) | block);

block	:	COLON INDENT (block_line_stmt)+ DEDENT;

block_inline	:	COLON (inline_expression)+;

inline_expression	:	attr_inline;

attr_inline	:	ATTRIBUTE_ID;

block_line_stmt	:	attr_stmt;

attr_stmt	:	ATTRIBUTE_ID (VALUE | OPEN_VALUE) NEWLINE;






