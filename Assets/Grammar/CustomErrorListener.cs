namespace anim
{
	using Antlr4.Runtime;
	using UnityEngine;

	public class CustomErrorListener : BaseErrorListener
	{
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol,
            int line, int charPositionInLine, string msg,
    RecognitionException e)
        {
            throw new ParseException("Invalid expression at line: " + line + ", character: " + charPositionInLine + ", Error message: " + msg);
        }
    }
}