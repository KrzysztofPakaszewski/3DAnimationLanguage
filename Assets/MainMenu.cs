using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using SimpleFileBrowser;

public class MainMenu : MonoBehaviour
{
    public GameObject popup;


    public Text SelectedFile;

    private string filename { get; set; }

    private string pathToFile { get; set; }

    public void playGame() {

        if (pathToFile != null)
        {
            ScriptAnimationHolder.Script = new TextAsset(File.ReadAllText(pathToFile));
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else {
            popup.SetActive(true);
            gameObject.active = false;

        }
    }

    public void quitGame() {
        Application.Quit();
    }

    public void pickScript() {
        StartCoroutine(ShowFilePicker());
    }

    public IEnumerator ShowFilePicker() {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files);

        if (FileBrowser.Success) {
            var result = FileBrowser.Result[0];
            pathToFile = result;
            filename = FileBrowserHelpers.GetFilename(result);
            SelectedFile.text = filename;
        }
    }
}