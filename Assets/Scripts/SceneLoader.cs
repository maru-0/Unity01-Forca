using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Classe que fará carga da Scene conforme um index
/// 
/// </summary>
///
public class SceneLoader : MonoBehaviour
{
    /* Método para carregar scene através do index */
    public void LoadOnClick(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    } 

}
