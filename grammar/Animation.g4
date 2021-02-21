grammar Animation;

// Parser Rules

// Initialization values

objectInitValue: OBJECT
    | SPHERE
    | CYLINDER
    | CUBE
    | PLANE
    ;



vectorInitValue :  LEFT_BRACKET x=numberValue COLON y=numberValue COLON z=numberValue RIGHT_BRACKET
       ;

groupInitValue :  LEFT_BRACKET args+=objectValue? RIGHT_BRACKET
       | LEFT_BRACKET args+=objectValue (COLON args+=objectValue)+ RIGHT_BRACKET
       ;


actionType: MOVE
    | ROTATE
    | TRANSFORM
    ;

objectValue: variable
    | objectInitValue
    | groupInitValue
    ;

vectorValue : variable
    | vectorInitValue
    ;


numberValue : expression
    ;

expression : term
    | expression ADD term
    | expression SUB term
    ;

term : factor
    | term MUL factor
    | term DIV factor
    ;

factor : NUMBER
    | SUB NUMBER
    | variable
    | LEFT_PAREN expression RIGHT_PAREN
    ;

variable: VARIABLE_NAME;

//

// Instructions

module: instr+=code+ EOF;

code: function
    | instruction;

instruction: block
    | command
    | if
    | while
    ;

block: parallel
    | sequence;

parallel: PAR_BEGIN instr+=instruction* PAR_END;

sequence: LEFT_BRACE instr+=instruction* RIGHT_BRACE;

command: assign SEMICOLON
    | action SEMICOLON
    | call SEMICOLON
    | wait SEMICOLON
    ;

type: OBJECT_TYPE
    | NUMBER_TYPE
    | VECTOR_TYPE
    | BOOLEAN_TYPE
    ;

wait: WAIT numberValue;

argument: type VARIABLE_NAME;

function: FUNCTION func_name=VARIABLE_NAME LEFT_PAREN ( args+=argument (COLON args+=argument)* )? RIGHT_PAREN body=sequence;


//Assign
objectAssign: OBJECT_TYPE name=VARIABLE_NAME ASSIGN value=objectInitValue;

groupAssign: OBJECT_TYPE name=VARIABLE_NAME ASSIGN value=groupInitValue;

numberAssign: NUMBER_TYPE name=VARIABLE_NAME ASSIGN value=numberValue;

vectorAssign: VECTOR_TYPE name=VARIABLE_NAME ASSIGN value=vectorInitValue;

booleanAssign: BOOLEAN_TYPE name=VARIABLE_NAME ASSIGN value=booleanValue;

allValues: objectInitValue
    | groupInitValue
    | numberValue
    | vectorInitValue
    | booleanValue
    ;

variableAssign: VARIABLE_NAME ASSIGN value=allValues;

altAssign: objectAssign
    | groupAssign
    | numberAssign
    | vectorAssign
    | variableAssign
    ;

assign : content=altAssign;

comparingOperation: EQUALS
    | NOT_EQUALS
    | LT
    | GT
    | LTE
    | GTE
    ;

comparing: left=numberValue op=comparingOperation right=numberValue;

altBoolean :
    | TRUE
    | FALSE
    | comparing
    | NOT value=altBoolean
    ;

exprBoolean : termBoolean
    | exprBoolean OR termBoolean
    ;

termBoolean : factBoolean
    | termBoolean AND factBoolean
    ;

factBoolean : altBoolean
    | variable
    | LEFT_PAREN exprBoolean RIGHT_PAREN
    ;

booleanValue: exprBoolean
    ;


action: act=actionType target=objectValue (FROM prev_val=vectorValue)? TO val=vectorValue (AT delay=numberValue)? IN time=numberValue;

if: IF LEFT_PAREN if_expr=booleanValue RIGHT_PAREN body=sequence else_expr=else?;

else: ELSE sequence
    | ELSE if
    ;

while: WHILE LEFT_PAREN test_expr=booleanValue RIGHT_PAREN body= sequence;

argValue: objectValue
    | vectorValue
    | booleanValue
    | numberValue
    ;

arguments: args+=argValue?
    | args+=argValue (COLON args+=argValue)+
    ;

call: VARIABLE_NAME LEFT_PAREN arguments RIGHT_PAREN;


// Lexer Rules

// Keywords

WAIT : 'wait';

FUNCTION : 'function';

IF : 'if';

ELSE : 'else';

WHILE : 'while';

FROM : 'from';

AT : 'at';

IN : 'in';

TO : 'to';

NUMBER_TYPE : 'Number';

GROUP_TYPE : 'Group';

VECTOR_TYPE : 'Vector';

OBJECT_TYPE : 'Object';

BOOLEAN_TYPE: 'Bool';

SPHERE : 'Sphere';

CUBE : 'Cube';

CYLINDER : 'Cylinder';

PLANE : 'Plane';

COLOR : 'Color';

ROTATE : 'rotate';

MOVE : 'move';

TRANSFORM : 'transform';

SET : 'set';

TRUE : 'true';

FALSE : 'false';


//separators


LEFT_BRACE : '{';

RIGHT_BRACE : '}';

LEFT_BRACKET : '[';

RIGHT_BRACKET : ']';

LEFT_PAREN : '(';

RIGHT_PAREN : ')';

SEMICOLON : ';';

COLON : ',';

PAR_BEGIN : '<>';

PAR_END : '</>';


// Operators

EQUALS : '==';

NOT_EQUALS : '!=';

ADD : '+';

SUB : '-';

MUL : '*';

DIV : '/';

ASSIGN : '=';

LT : '<';

GT : '>';

LTE : '<=';

GTE : '>=';

AND : '&&';

OR : '||';

NOT: '!';

// Number Fragments

fragment Digit : [0-9];

fragment NonZero : [1-9];

fragment Zero : '0';

fragment PositiveNumber : Zero
       | NonZero Digit*
       ;


// values for types

NUMBER : (Zero | (NonZero Digit*)) ('.' Digit+)?;

//variable name

fragment Letter : [a-zA-Z_] | '-';

fragment FirstLetter : [a-z];

VARIABLE_NAME : FirstLetter (Letter | Digit)*;

QUOTE : '"'
;

OBJECT : QUOTE FileLetter+ QUOTE;

fragment FileLetter : Letter | '/' | '.';

// skip white space

WHITE_SPACE  :  [ \t\r\n\u000C]+ -> skip;

UNKNOWN : .;