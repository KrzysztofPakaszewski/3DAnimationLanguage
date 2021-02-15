namespace anim {

	using System.Collections;
	using System.Collections.Generic;

	public class Variable
	{
		public static string OBJECT = "Object";

		public static string NUMBER = "Number";

		public static string VECTOR = "Vector";

		public static string BOOLEAN = "Boolean";

		string type;

		object value;

		string name;

		public Variable(string type, object value, string name ="")
		{
			this.type = type;
			this.value = value;
			this.name = name;
		}


		public string GetType() { return type; }

		public object GetValue() { return value; }

		public string GetName() { return name; }
	}

	public class Function
	{
		List<Variable> variables = new List<Variable>();

		AnimationParser.SequenceContext body;

		public Function(AnimationParser.SequenceContext body) {
			this.body = body;
		}

		public void AddVariable(Variable variable) {
			this.variables.Add(variable);
		}

		public List<Variable> GetVariables() { return variables; }

		public AnimationParser.SequenceContext GetBody() { return body;  }
	}
}