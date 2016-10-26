lexer grammar MalinaLexer;

tokens { INDENT, DEDENT, NEWLINE, OPEN_VALUE_ML, EQUAL, DBL_EQUAL, DQS, DQS_ML}

WS				:	WsSpaces	-> skip;

INDENT_DEDENT		:	((Eol Spaces)+) {IndentDedent();};

COLON				:	':';

LPAREN				:	'('	{EnterWsa();} -> skip;
RPAREN				:	')'	{ExitWsa();}  -> skip;
COMMA				:	',';

NAMESPACE_ID		:	'#' ShortName;
DOCUMENT_ID			:	'!' Name;
ALIAS_DEF_ID		:	'!$' Name;
SCOPE_ID			:   '#' FullName |'#.' ShortName ;
ATTRIBUTE_ID		:	'@' Name;
ALIAS_ID			:	'$' Name;
PARAMETER_ID		:	'%' Name;
ARGUMENT_ID			:	'.' Name;
EQUAL				:	'=';
DBL_EQUAL			:	'==';
ELEMENT_ID			:	ShortName | FullName;

VALUE_BEGIN			:	EQUAL Spaces {Emit(EQUAL);StartNewMultliLineToken();} -> pushMode(IN_VALUE);
OPEN_VALUE_BEGIN	:	DBL_EQUAL	Spaces {Emit(DBL_EQUAL);StartNewMultliLineToken();} -> pushMode(IN_VALUE);

mode IN_VALUE;
	//Parameter or Alias assignment
	OBJECT_VALUE	: 
					(	PARAMETER_ID {Emit(PARAMETER_ID);}						
					|	ALIAS_ID {Emit(ALIAS_ID);}
					)		-> popMode;

	//Open string and Multi Line Open String
	OPEN_VALUE_EOL		:	(Eol Spaces)+ '=='?  {OsIndentDedent();}; //End of Open String Line or End of Open String
	OPEN_VALUE			:	~[$%"\'\r\n](~[\r\n])* {EndOpenValueIfEof();}; //Open string content can't start with [$%"\'\r\n]
	DQS					:	'"' (~["\r\n] | '""')+ '"' -> popMode;
	DQS_ML				:	'"' {StartDqs();} -> skip, pushMode(IN_DQS);

mode IN_DQS;
	//Double Quoted String
	DQS_VALUE		:	(~["\r\n] | '""')+ {EndDqsIfEof();};

	DQS_VALUE_EOL	:	(Eol Spaces)+ {DqIndentDedent();}; //End of DQS Line or End of DQS

	DQS_END			:	'"' {EndDqs();};
	//Double quoted string multiline
	//DQS_ML			:	
	//				(	'"' (~["] | '""')* '"'	
	//				)		-> popMode;

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

fragment	FullName		:	(ShortName '.')+ ShortName
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