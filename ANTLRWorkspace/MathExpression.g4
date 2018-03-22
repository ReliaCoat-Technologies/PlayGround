grammar MathExpression;

// Parsers

expression
    : (parentheses | term) (('+'|'-') (parentheses | term))*
    ;
term
    : (parentheses | factor) (('*'|'/') (parentheses | factor))*
    ;
factor
    : (parentheses | ATOM) (('^') (parentheses | ATOM))*
    ;
parentheses
    : (OPENP (expression)+ CLOSEP)
    ;
/*
 * Lexers - Individual characters into tokens
 * Analyzed in order of appearence
 */

fragment LETTER
    : [a-zA-Z]
    ;
fragment DIGIT
    : [0-9]
    ;

ATOM
    : (WORD | DOUBLE)
    ;
OPENP
    : '('
    ;
CLOSEP
    : ')'
    ;
DOUBLE
    : '-'?DIGIT+('.'DIGIT+)?('E'DIGIT+)?
    ;
INTEGER
    : DIGIT+
    ;
WORD
    : LETTER+
    ;
NEWLINE
    : ('\r'? '\n' | 'r')+ -> skip
    ;
WHITESPACE
    : (' '|'\t')+ -> skip
    ;