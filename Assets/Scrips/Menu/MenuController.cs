using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    public string SceneName;
    public GameObject contentPanelSettings;
    [SerializeField] private bool deletePlayerLog = true;

    private readonly Regex gameDataRegex = new Regex(@"^game_data_\d{8}_\d{6}\.csv$");

    public void StartGame()
    {
        PlayerManager.SaveAgents();
        Simulation.isSimulation = false;
        SceneManager.LoadScene(SceneName);
    }

    public void ExitApplication()
    {
        PlayerManager.SaveAgents();
        Application.Quit();
    }

    public void ShowSettings()
    {
        bool state = contentPanelSettings.gameObject.activeInHierarchy;
        contentPanelSettings.SetActive(!state);
    }
    public void StartSimulation()
    {
        PlayerManager.SaveAgents();
        Simulation.isSimulation = true;
        SceneManager.LoadScene(SceneName);
    }

    public void SafeClearData()
    {
        string persistentDataPath = Application.persistentDataPath;

        if (!Directory.Exists(persistentDataPath))
        {
            Debug.LogWarning("Persistent data path doesn't exist!");
            return;
        }

        try
        {
            int deletedFilesCount = 0;

            foreach (string file in Directory.GetFiles(persistentDataPath))
            {
                string fileName = Path.GetFileName(file);

                bool shouldDelete = false;

                if (gameDataRegex.IsMatch(fileName))
                {
                    shouldDelete = true;
                }
                else if (deletePlayerLog && fileName.Equals("Player.log"))
                {
                    shouldDelete = true;
                }

                if (shouldDelete)
                {
                    try
                    {
                        File.Delete(file);
                        deletedFilesCount++;
                    }
                    catch (System.Exception e)
                    {
                    }
                }
            }


        }
        catch (System.Exception e)
        {
        }
    }

}
