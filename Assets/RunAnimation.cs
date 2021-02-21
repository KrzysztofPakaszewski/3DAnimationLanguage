using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using UnityEngine;
using anim;
using UnityEngine.SceneManagement;

public class RunAnimation : MonoBehaviour
{

    public ErrorGame errorPopUp;


    // Start is called before the first frame update
    void Start()
    {
        try
        {
            AntlrInputStream stream = new AntlrInputStream(ScriptAnimationHolder.Script.text);
            AnimationLexer lexer = new AnimationLexer(stream);
            CommonTokenStream tokenStream = new CommonTokenStream(lexer);
            AnimationParser parser = new AnimationParser(tokenStream);
            parser.AddErrorListener(new CustomErrorListener());

            AnimationParser.ModuleContext moduleContext = parser.module();

            CustomVisitor visitor = new CustomVisitor();
            visitor.Visit(moduleContext);
        }
        catch (Exception e) {
            errorPopUp.ErrorMessage.text = e.Message;
            errorPopUp.SetActive(true);
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.Escape))
        {
            goBackToMenu();
        }

    }

    private void goBackToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
