using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    private Dictionary<string,string> solutions;

    [System.Serializable]
    public class Solution
    {
        public string answer;
        public string question;
    }

    [System.Serializable]
    public class SolutionsData
    {
        public Solution[] solutionsdata;
    }

    [Header("Data")]
    [SerializeField] private TextAsset jsonData;

    [Header("Managers")]
    [SerializeField] private UIManager UImanager;

    [Header("Settings")]
    [SerializeField] private int numAnswers = 3;
    [SerializeField] private float questionSecondsOnScreen = 2f;
    [SerializeField] private float fadeSeconds = 2f;

    private string actualsolution;

    private int errors;
    private int success;

    private int roundErrors;

    public void InputPlayer(Text answer)
    {
        if (answer.text == actualsolution)
        {
            AddSuccess();
            UImanager.MarkCorrect(answer.transform.parent.gameObject);
            FinishRound();
        }
        else
        {
            if (roundErrors == 0)
            {
                roundErrors++;
                AddError();
                UImanager.MarkIncorrect(answer.transform.parent.gameObject);
                StartCoroutine(UImanager.FadeOutOneAnswer_CO(answer.transform.parent.gameObject, fadeSeconds));
            }
            else
            {
                AddError();
                UImanager.MarkSolution(actualsolution);
                FinishRound();
            }
        }

    }

    void Awake()
    {
        ChargeSolutionsData();
        Initialization();
    }

    private void ChargeSolutionsData()
    {
        solutions = new Dictionary<string, string>();
        var data = JsonUtility.FromJson<SolutionsData>(jsonData.text);
        foreach(Solution solution in data.solutionsdata)
        {
            solutions.Add(solution.answer, solution.question);
        }
    }

    private void Initialization()
    {
        errors = 0;
        success = 0;
        roundErrors = 0;

        UImanager.SetErrors(errors);
        UImanager.SetSucces(success);
    }

    void Start()
    {
        StartCoroutine(CreateQuestion_CO());
    }

    private IEnumerator CreateQuestion_CO()
    {
        SelectQuestion();
        UImanager.SetQuestionText(solutions[actualsolution]);
        yield return StartCoroutine(UImanager.AnimationQuestion_CO(fadeSeconds, questionSecondsOnScreen));
        AnswerList();
    }

    private void SelectQuestion()
    {
        var selectsolution = Random.Range(0, solutions.Count);
        actualsolution = solutions.ElementAt(selectsolution).Key;
    }

    private void AnswerList()
    {
        List<string> optionlist = CreateAnswerList();
        UImanager.CreateAnswersButtons(optionlist);
        StartCoroutine(UImanager.FadeInAnswersGrid_CO(fadeSeconds));
    }

    private List<string> CreateAnswerList()
    {
        var keylist = new List<string>(solutions.Keys);
        keylist.Remove(actualsolution);

        var optionlist = new List<string>();

        for (int i = 0; i < numAnswers; i++)
        {
            var randomanswer = Random.Range(0, keylist.Count);
            optionlist.Add(keylist[randomanswer]);
            keylist.RemoveAt(randomanswer);
        }

        var randomindex = Random.Range(0, numAnswers);
        optionlist[randomindex] = actualsolution;

        return optionlist;
    }

    private void AddSuccess()
    {
        ++success;
        UImanager.SetSucces(success);
    }

    private void AddError()
    {
        ++errors;
        UImanager.SetErrors(errors);
    }

    private void FinishRound()
    {
        StartCoroutine(NextRound_CO());
    }

    private IEnumerator NextRound_CO()
    {
        yield return StartCoroutine(UImanager.FadeOutAnswersGrid_CO(fadeSeconds));
        UImanager.CleanButtons();
        NewRound();
    }

    private void NewRound()
    {
        roundErrors = 0;
        StartCoroutine(CreateQuestion_CO());
    }

    


}
