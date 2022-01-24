using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] GameManager gameManager;

    [Header("Components")]
    [SerializeField] Text question;
    [SerializeField] GameObject answersGrid;
    [SerializeField] GameObject answerPrefab;
    [SerializeField] Text errorsText;
    [SerializeField] Text successText;

    CanvasGroup canvasgroupAnswers;

    private void Awake()
    {
        question.CrossFadeAlpha(0, 0, false);
        canvasgroupAnswers = answersGrid.GetComponent<CanvasGroup>();
        canvasgroupAnswers.alpha = 0;
    }

    public void SetQuestionText(string selectedQuestion)
    {
        question.text = selectedQuestion;
    }

    public IEnumerator AnimationQuestion_CO(float fadeSeconds, float SecondsOnScreen)
    {
        FadeInQuestion(fadeSeconds);
        yield return new WaitForSeconds(fadeSeconds);
        yield return new WaitForSeconds(SecondsOnScreen);
        FadeoutQuestion(fadeSeconds);
        yield return new WaitForSeconds(fadeSeconds);
    }

    public void FadeInQuestion(float fadeSeconds)
    {
        question.CrossFadeAlpha(1, fadeSeconds, false);
    }

    public void FadeoutQuestion(float fadeSeconds)
    {
        question.CrossFadeAlpha(0, fadeSeconds, false);
    }

    public void CreateAnswersButtons(List<string> listAnswers)
    {
        for(int i = 0; i < listAnswers.Count; i++)
        {
            GameObject answerButton = Instantiate(answerPrefab, answersGrid.transform, true);
            var textButton = answerButton.transform.GetChild(0).GetComponent<Text>();
            textButton.text = listAnswers[i];
            answerButton.GetComponent<Button>().onClick.AddListener(() => InputClick(textButton));
        }

        DisableAnswerButtons();
    }

    public void EnableAnswerButtons()
    {
        canvasgroupAnswers.blocksRaycasts = true;
    }
    public void DisableAnswerButtons()
    {
        canvasgroupAnswers.blocksRaycasts = false;
    }

    public IEnumerator FadeInAnswersGrid_CO(float fadeSeconds)
    {
        for (float i = 0f; i <= fadeSeconds; i += Time.deltaTime)
        {
            canvasgroupAnswers.alpha = i;
            yield return null;
        }
        canvasgroupAnswers.alpha = 1f;

        EnableAnswerButtons();
    }

    public IEnumerator FadeOutAnswersGrid_CO(float fadeSeconds)
    {
        for (float i = fadeSeconds; i >= 0f; i -= Time.deltaTime)
        {
            canvasgroupAnswers.alpha = i;
            yield return null;
        }
        canvasgroupAnswers.alpha = 0f;
    }

    public IEnumerator FadeOutOneAnswer_CO(GameObject answer, float fadeSeconds)
    {
        var canvasgroupAnswer = answer.GetComponent<CanvasGroup>();
        for (float i = fadeSeconds; i >= 0f; i -= Time.deltaTime)
        {
            canvasgroupAnswer.alpha = i;
            yield return null;
        }
        answer.GetComponent<Button>().interactable = false;
        canvasgroupAnswer.alpha = 0f;

        EnableAnswerButtons();
    }

    public void MarkSolution(string solution)
    {
        foreach (Transform answerButton in answersGrid.transform)
        {
            var answerText = answerButton.GetChild(0).GetComponent<Text>();
            if (solution == answerText.text) MarkCorrect(answerButton.gameObject);
            else MarkIncorrect(answerButton.gameObject);
        }
    }

    public void MarkCorrect(GameObject answerButton)
    {
        answerButton.GetComponent<Image>().color = Color.green;
    }

    public void MarkIncorrect(GameObject answerButton)
    {
        answerButton.GetComponent<Image>().color = Color.red;
    }

    public void SetSucces(int success)
    {
        successText.text = success.ToString();
    }

    public void SetErrors(int errors)
    {
        errorsText.text = errors.ToString();
    }

    public void CleanButtons()
    {
        foreach (Transform answerButton in answersGrid.transform)
        {
            Destroy(answerButton.gameObject);
        }
    }
    public void InputClick(Text answerText)
    {
        DisableAnswerButtons();
        gameManager.InputPlayer(answerText);
    }

}
