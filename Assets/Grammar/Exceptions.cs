namespace anim {

    using System;

    public class InvalidObjectValue : Exception {
        public InvalidObjectValue() : base("Invalid object value") { }
    }

    public class InvalidActionType : Exception {
        public InvalidActionType() : base("Invalid action type") { }
    }

    public class VariableUndefined : Exception {
        public VariableUndefined() : base("Variable undefined") { }
    }

    public class InvalidObjectType : Exception {
        public InvalidObjectType() : base("Invalid type") { }
    }

    public class InvalidOperation : Exception {
        public InvalidOperation() : base("Invalid operation") { }
    }

    public class FunctionUndefined : Exception
    {
        public FunctionUndefined() : base("Function undefined") { }
    }

    public class InvalidArguments : Exception { 
        public InvalidArguments(): base("Invalid arguments for functions"){}
    }

    public class ParseException : Exception {
        public ParseException(String e) : base(e) { }
    }

}