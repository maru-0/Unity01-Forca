using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private int       numErros;             // Quantidade de tentativas válidas resultantes em erros
    private int       maxNumErros;          // Quantidade máxima de erros antes do Game Over
    private string    letrasIncorretas;     // String que armazena as letras das tentativas incorretas
    public GameObject letra;                // Prefab da letra no game 
    public GameObject centro;               // Objeto de texto que indica o centro da tela  

    private string[]  palavrasOcultas;      // Lista de palavras d qual será retirada a palavra oculta     
    private string    palavraOculta;        // Palavra a ser adivinhada

    private int       tamanhoPalavraOculta; // Tamanho da palavra oculta
    char[]            letrasOcultas;        // Letras da palavra oculta
    bool[]            letrasDescobertas;    // Indicador de quais letras foram descobertas
    int               numLetrasDescobertas; // Número de letras já descobertas  

    // Start is called before the first frame update
    void Start()
    {
        numErros             = 0;
        maxNumErros          = 5;
        centro               = GameObject.Find("centroDaTela");   // Vincula o GameObject "centroDaTela" ao atributo "centro"
        letrasIncorretas     = "";
        numLetrasDescobertas = 0;

        LerPalavrasDoArquivo();
        InitGame();
        InitLetras();
    }

    // Update is called once per frame
    void Update()
    {
       CheckTeclado();               // Checa teclas pressionadas no teclado
       UpdateVitoriasConsecutivas(); // Atualiza elemento da UI das vitorias consecutivas
       UpdateNumErros();             // Atualiza elemento da UI do número de erros

       if(numErros == maxNumErros) SceneManager.LoadScene("GameOver");                      // Checa se houve Game Over e caso sim carrega a scene correspondente
       
       // Checa se adivinhou palavra e caso sim carrega a scene correspondente depois de atualizar a quantidade de vitorias
       if(numLetrasDescobertas == tamanhoPalavraOculta){ 
           PlayerPrefs.SetInt("winStreak", PlayerPrefs.GetInt("winStreak") + 1);
           SceneManager.LoadScene("VictoryScreen"); 
        }
    }

    // Inicializa o jogo definindo a palavra oculta de forma aleatória a partir da lista de palavras possíveis e inicializando variáveis relacionadas
    void InitGame()
    {
        palavraOculta        = palavrasOcultas[Random.Range(0, palavrasOcultas.Length)].ToUpper(); // Definição da palavra oculta a ser descoberta, deixando em upper case
        tamanhoPalavraOculta = palavraOculta.Length;           // Determinando o número de letras na palavra oculta
        letrasOcultas        = new char[tamanhoPalavraOculta]; // Instanciar array de char com as letras da palavra
        letrasOcultas        = palavraOculta.ToCharArray();    // Copia a palavra oculta parra o array de letras
        letrasDescobertas    = new bool[tamanhoPalavraOculta]; // instanciar array de bool que indica letras descobertas
    }


    // Inicializa GameObjects referentes as letras da palavra oculta como filhos do GameObject "Canvas", baseando-se no prefab já vinculado no atributo "letra"
    void InitLetras()
    {
        for(int i = 0; i < tamanhoPalavraOculta; i++){
            // Espaça as letras em 80
            Vector3 novaPosicao = new Vector3(centro.transform.position.x + (i - tamanhoPalavraOculta/2.0f ) * 80, 
                                              centro.transform.position.y, 
                                              centro.transform.position.z);
            GameObject l        = (GameObject)Instantiate(letra, novaPosicao, Quaternion.identity);
            l.name              = "letra " + (i+1);                    // Nomeia cada GameObject das letras na forma "letra x", começando a contar do 1
            l.transform.SetParent(GameObject.Find("Canvas").transform);
        }
    }

    // Verifica inputs do teclado. Caso seja letra, verifica se está presente no atributo "palavraOculta"
    void CheckTeclado()
    {
        if(Input.anyKeyDown){
            char letraTeclada   = Input.inputString.ToCharArray()[0];
            int letraTecladaInt = System.Convert.ToInt32(letraTeclada);

            // Checa se é uma letra
            if(letraTecladaInt >= 97 && letraTecladaInt <= 122){
                bool flagErro = true;     // Flag que indica se houve erro na tentativa. Inicia como true, é mudada para false no caso de acerto
                // Percorre a palavra
                for(int i = 0; i < tamanhoPalavraOculta; i++){
                    // Caso seja uma letra ainda não descoberta, checa se a letra digitada corresponde a letra atual
                    if(!letrasDescobertas[i]){
                        letraTeclada = System.Char.ToUpper(letraTeclada);

                        if(letrasOcultas[i] == letraTeclada){
                            // Caso tenha descoberto uma nova letra, atualiza o atributo "letrasDescobertas" e o GameObject correspondente
                            letrasDescobertas[i]                                        = true;
                            GameObject.Find("letra " + (i+1)).GetComponent<Text>().text = letraTeclada.ToString();
                            flagErro                                                    = false;
                            numLetrasDescobertas++;
                        }
                    }
                }   
                // Incrementa no caso de erro, e acrescenta a letra da tentativa na lista
                if(flagErro) {
                    numErros++; 
                    letrasIncorretas += letraTeclada;
                }                
            }
        }
    }

    // Atualiza o GameObject numErros para exibir na tela a quantidade de erros e as letras das tentativas incorretas
    void UpdateNumErros(){
        GameObject.Find("numErros").GetComponent<Text>().text = "Erros: " + numErros + " | " + maxNumErros + "\n" +
                                                                  letrasIncorretas;
    }

    /* Atualiza GameObject vitoriasConsecutivas para exibir na tela*/
    void UpdateVitoriasConsecutivas()
    {
        GameObject.Find("vitoriasConsecutivas").GetComponent<Text>().text = "Vitorias consecutivas: " + PlayerPrefs.GetInt("winStreak");
    }

    /* Setta o atributo "palavrasOcultas" com as palavras lidas de um arquivo*/
    void LerPalavrasDoArquivo()
    {
        TextAsset t1 = (TextAsset)Resources.Load("palavras", typeof(TextAsset));
        palavrasOcultas = t1.text.Split(' ');
    }

}

