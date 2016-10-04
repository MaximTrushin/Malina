lexer grammar MalinaLexer;
@header {
  using System.Collections.Generic;
}

@lexer::members {
  
  public List<IToken> Invalid = new List<IToken>();
}


WS				:	WsSpaces	-> skip;

INDENT_DEDENT		:	Eol Spaces;

COLON				:	':';

LPAREN				:	'('	;
RPAREN				:	')'	;
COMMA				:	',';

NAMESPACE_ID		:	'#' ShortName;
DOCUMENT_ID			:	'!' Name;
ALIAS_DEF_ID		:	'!$' Name;
SCOPE_ID			:   '#' FullName;
ATTRIBUTE_ID		:	'@' Name;
ALIAS_ID			:	'$' Name;
PARAMETER_ID		:	'%' Name;
ARGUMENT_ID			:	'.' Name;

FULL_ID				:	FullName;
SHORT_ID			:	ShortName;

VALUE_BEGIN			:	'='	[ \t]* -> pushMode(IN_VALUE);


INVALID : . { if (Invalid.Count < 100) Invalid.Add(Token); };

mode IN_VALUE;
	OBJECT_VALUE	:
					(	PARAMETER_ID						
					|	ALIAS_ID
					|	INVALID					
					)		-> popMode;
	VALUE			:	
					(	'"' (~('"') | '""')* '"'	
					)		-> popMode;
	O_VALUE_BEGIN	:	~[$%"\'`] ~[)\r\n]*;


fragment	Eol				:	( '\r'? '\n' )
							;

fragment	Spaces			:	([ \t] | Comment)*
							;

fragment	WsSpaces			:	([ \t] | Comment)+
							;


fragment	Comment			:	BlockComment
							|	LineComment
							;

fragment	BlockComment	:   '/*' .*? '*/'
							;

fragment	LineComment		:   '//' ~[\r\n]*
							;

fragment	Name			:	FullName | ShortName
							;

fragment	FullName		:	(NameStartChar NameChar* '.')+ NameStartChar NameChar*
							;

fragment	ShortName		:	NameStartChar NameChar*
							;

//http://www.w3.org/TR/REC-xml/							
//[4a]   	NameChar	   ::=   	NameStartChar | "-" | "." | [0-9] | #xB7 | [#x0300-#x036F] | [#x203F-#x2040]
fragment	NameChar		:   NameStartChar
							|	'..'
							|   '-' | '_' | Digit 
							|   '\u00B7'
							|   '\u0300'..'\u036F'
							|   '\u203F'..'\u2040'
							;

//http://www.w3.org/TR/REC-xml/
//[4]   	NameStartChar	   ::=   	":" | [A-Z] | "_" | [a-z] 
//											| [#xC0-#xD6] | [#xD8-#xF6] | [#xF8-#x2FF] | [#x370-#x37D] | [#x37F-#x1FFF] | [#x200C-#x200D] | [#x2070-#x218F] 
//											| [#x2C00-#x2FEF] | [#x3001-#xD7FF] | [#xF900-#xFDCF] | [#xFDF0-#xFFFD] | [#x10000-#xEFFFF]
fragment	NameStartChar	:   [a-zA-Z]
							|	'_'
							|   '\u00c0'..'\u00d6' 
							|   '\u00d8'..'\u00f6' 
							|   '\u00f8'..'\u02ff' 
							|   '\u0370'..'\u037d' 
							|   '\u037f'..'\u1fff' 
							|   '\u200c'..'\u200d' 
							|   '\u2070'..'\u218F' 
							|   '\u2C00'..'\u2FEF' 
							|   '\u3001'..'\uD7FF' 
							|   '\uF900'..'\uFDCF' 
							|   '\uFDF0'..'\uFFFD'
//							|   '\u00010000'..'\u000effff' utf-32 not supported by ANTLR
							;

fragment	Digit			:   [0-9]
							;