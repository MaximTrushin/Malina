lexer grammar MalinaLexer;

tokens { INDENT, DEDENT, NEWLINE, FREE_OPEN_STRING, OPEN_STRING_ML, FREE_OPEN_STRING_ML, EQUAL, DBL_EQUAL, DQS, DQS_ML, COLON, SQS}


INDENT_DEDENT		:	(Eol Spaces) {IndentDedent();};

ARRAY_ITEM			:	':';

LPAREN				:	'('	{EnterWsa();} -> skip;
RPAREN				:	')' Spaces	{ExitWsa();}  -> skip;
COMMA				:	',';


NAMESPACE_ID		:	'#' ShortName;
DOCUMENT_ID			:	'!' Name ':'? {EmitIdWithColon(DOCUMENT_ID);};
ALIAS_DEF_ID		:	'!$' Name ':'? {EmitIdWithColon(ALIAS_DEF_ID);};
SCOPE_ID			:   ((('#' FullName |'#.' ShortName | '#' ShortName) ':'?) | '#:') {EmitIdWithColon(SCOPE_ID);};
ATTRIBUTE_ID		:	'@' ((ShortName '.')? ShortName) {EmitIdWithColon(ATTRIBUTE_ID);};
ALIAS_ID			:	'$' Name ':'? {EmitIdWithColon(ALIAS_ID);};
PARAMETER_ID		:	'%' ShortName ':'? {EmitIdWithColon(PARAMETER_ID);};
ARGUMENT_ID			:	'.' ShortName ':'? {EmitIdWithColon(ARGUMENT_ID);};
ELEMENT_ID			:	((ShortName '.')? ShortName) ':'? {EmitIdWithColon(ELEMENT_ID);};

VALUE_BEGIN			:	'=' [ \t]* {Emit(EQUAL);StartNewMultiLineToken();} -> pushMode(IN_VALUE);
OPEN_VALUE_BEGIN	:	'=='	[ \t]* {Emit(DBL_EQUAL);StartNewMultiLineToken();} -> pushMode(IN_FREE_VALUE);

EMPTY_OBJECT		:	'()';
EMPTY_ARRAY			:	'(:)';

WS				:	WsSpaces	-> skip;

mode IN_VALUE;
	//Parameter or Alias assignment
	OBJECT_VALUE	: 
					(	(PARAMETER_ID {EmitIdWithColon(PARAMETER_ID);})						
					|	(ALIAS_ID {EmitIdWithColon(ALIAS_ID);})
					)		-> popMode;

	//Open string and Multi Line Open String
	OPEN_STRING_EOL		:	OpenStringEol {OsIndentDedent(OPEN_STRING, OPEN_STRING_ML);};
	JSON_BOOLEAN		:	('true' | 'false') {ProcessOpenStringLine(JSON_BOOLEAN);};
	JSON_NULL		:	'null' {ProcessOpenStringLine(JSON_NULL);};
	JSON_NUMBER		:	('-'? Int '.' [0-9] + Exp? | '-'? Int Exp | '-'? Int) {ProcessOpenStringLine(JSON_NUMBER);};
	OPEN_STRING			:	OpenStringStart {ProcessOpenStringLine(OPEN_STRING);};
	
	//Double Qoute String (DQS) and Multiline DQS
	DQS					:	'"' (~["\r\n] | '""')+ '"' -> popMode;
	DQS_ML				:	'"' (~["\r\n] | '""')* {StartDqsMl();};

	//Single Qoute String (SQS) and Multiline SQS
	SQS					: '\'' {StartSqs();}  -> pushMode(IN_SQS);


mode IN_FREE_VALUE;
	//Open string and Multi Line Open String
	FREE_OPEN_STRING_EOL		:	OpenStringEol {OsIndentDedent(FREE_OPEN_STRING, FREE_OPEN_STRING_ML);};
	FREE_OPEN_STRING			:	OpenString {ProcessOpenStringLine(OPEN_STRING);};

mode IN_OS;
	IN_OPEN_STRING_EOL		:	OpenStringEol {OsIndentDedent(OPEN_STRING, OPEN_STRING_ML);};
	IN_OPEN_STRING			:	OpenString {ProcessOpenStringLine(OPEN_STRING);};



mode IN_DQS;
	//Double Quoted String
	DQS_VALUE		:	(~["\r\n] | '""')+ {EndDqsIfEofOrWsa();};

	DQS_VALUE_EOL	:	(Eol Spaces)+ {DqIndentDedent();}; //End of DQS Line or End of DQS

	DQS_END			:	'"' {EndDqs();};

//Single Quoted String (one line and multiline)
mode IN_SQS;
	
	SQS_JSON_BOOLEAN	:	'true' | 'false';
	SQS_JSON_NULL		:	'null';
	SQS_JSON_NUMBER		:	('-'? Int '.' [0-9] + Exp? | '-'? Int Exp | '-'? Int);
	SQS_ESCAPE			:	EscSeq | '$$' | '\'\'' | SqsEscapeCode;
	INTERPOLATION		:	('$' | '%') (Name | ('(' [ \t]* Name [ \t]* ')' ) )?;
	SQS_VALUE			:	(~[$%\'\r\n\\])+ {EndSqsIfEofOrWsa();}; //Non-gready rule gives priority to other interpolation tokens
	SQS_EOL				:	(Eol Spaces)+ {SqIndentDedent();}; //Ends SQS Line or whole SQS if dedent or EOF.
	SQS_END				:	'\'' -> popMode, popMode;

fragment	EscSeq	: ('\\' ('\"'|'\''|'\\'|'/'|'$'|'b'|'f'|'n'|'r'|'t'|'v')) | UnicodeEsc;
fragment	UnicodeEsc	: '\\' 'u' HexDigit HexDigit HexDigit HexDigit;

fragment	SqsEscapeCode	:	'$' (
										EscapeDecimalNumber | 
										('(' [ \t]* EscapeDecimalNumber [ \t]* ')' ) | 
										('%' EscapeHexNumber) | 
										('(' [ \t]* '%' EscapeHexNumber [ \t]* ')' )
								);

fragment	EscapeDecimalNumber	:	Digit Digit? Digit? Digit? Digit?;

fragment	EscapeHexNumber		:	HexDigit HexDigit? HexDigit? HexDigit?;

fragment	Eol				:	( '\r'? '\n' )
							;

fragment	Spaces			:	([ \t] | Comment)*
							;

fragment	WsSpaces			:	([ \t] | Comment)+
							;


fragment	Comment			:	'//' ~[\r\n]*;

fragment	Name			:	LongName | ShortName
							;

fragment	FullName		:	(ShortName '.') ShortName
							;

fragment	ShortName		:	NameStartChar NameChar*
							;

fragment	LongName		:	(ShortName '.')+ ShortName
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
							|   '\u00010000'..'\u000effff'
							;

fragment	Digit			:   [0-9]
							;

fragment	HexDigit		:   Digit | [a-fA-F]
							;

fragment OpenStringEol	:	(Eol [ \t]*)+ '=='?; //End of Open String Line or End of Open String
fragment OpenStringStart	:	~[$%"\'\r\n](~[\r\n])*; //Open string content can't start with [$%"\'\r\n]
fragment OpenString	:	~[\r\n]+; //Open string content
fragment Int
   : '0' | [1-9] [0-9]*
   ;
// no leading zeros
fragment Exp
   : [Ee] [+\-]? Int
   ;