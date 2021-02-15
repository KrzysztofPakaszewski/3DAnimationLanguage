using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using UnityEngine;
using anim;

public class RunAnimation : MonoBehaviour
{

    public TextAsset asset;

    // Start is called before the first frame update
    void Start()
    {
        AntlrInputStream stream = new AntlrInputStream(asset.text);
        AnimationLexer lexer = new AnimationLexer(stream);
        CommonTokenStream tokenStream = new CommonTokenStream(lexer);
        AnimationParser parser = new AnimationParser(tokenStream);

        AnimationParser.ModuleContext moduleContext = parser.module();

        CustomVisitor visitor = new CustomVisitor();
        visitor.Visit(moduleContext);

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
