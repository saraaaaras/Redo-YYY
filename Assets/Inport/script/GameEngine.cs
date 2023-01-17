using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;


public class GameEngine : MonoBehaviour
{
    public Button[] diceButton;
    public Button[] conditionButton;
    public Sprite[] diceFacesSprite;
    private Sprite[] originalRolledResultSprite;
    public Sprite[] updatedRolledResultSprite; 
    public Sprite QuestionMarkSprite;
    public int[] rolledResult;
    public int[] eachDiceFaceShownTimes;

    public Button thisConditionButton;

    public Button rollButton;
    public TMP_Text totalScoreText;
    private int maxPressRollButtonTimes = 3;
    public static int currentPressRollButtonTimes = 0;
    public int howManyDice = 5;
    public int howManyFacePerDice = 6;

    public int howManyCondition = 14;
    
    public Color darkGreyColor = new Color(0.5f, 0.5f, 0.5f, 1f);


    [Header("Condition fix score")]
    public int fullHouseScore = 25;
    public int smallStraightScore = 30;
    public int largeStraightScore = 40;
    public int YahtzeeScore = 50;
    public int YahtzeeBonusScore = 100;
    public int timesHitYahtzee = 0;

    public static int bonusScore = -63;

    public TMP_Text bonusText;

    public static int totalScore = 0;

    public static bool notHitBonus = true;

    public static int timePressedCondition = 0;  

    [Header("each Condition result")]
    public static int[] eachConditionResult;





    void Start()
    {
        rolledResult = new int[howManyDice]; 
        originalRolledResultSprite = new Sprite[howManyDice];
        updatedRolledResultSprite = new Sprite[howManyDice];
        eachDiceFaceShownTimes = new int[howManyFacePerDice];
        eachConditionResult = new int[howManyCondition]; 
        
        for (int i = 0; i < howManyDice; i++)  //make the updatedRolledResultSprite[] are all ? sprite inside the array
        {
            updatedRolledResultSprite[i] = QuestionMarkSprite;
        }
    }

    public void PressRollDice()
    { 
        currentPressRollButtonTimes++;
        Debug.Log("current Press Roll Button Times"+currentPressRollButtonTimes);

        RollAndSaveTheResult(); //when pressed the roll button, it will rnd the int array
        ChangeEachDicesSprite(); //change the dice sprite by referring to the result
        ForcePlayerNotClickTheScoreButton(true,true);

        foreach (Button conditionButton in conditionButton) // reset all the unclicked condition button to enabled
        {
            if(conditionButton.GetComponentInChildren<TMP_Text>().color != Color.white)
            {
                conditionButton.enabled = true;
            }
        }

        CheckAllConditionFitOrNot(); //check all condition;

        if(currentPressRollButtonTimes == maxPressRollButtonTimes) //if times pressing roll button up to limit, player cannot press the roll & dice buttons anymore (force player need to click one of the score button)
        {
            ForcePlayerNotClickTheScoreButton(false,false);
            currentPressRollButtonTimes = 0;
        }
    }



    public void RollAndSaveTheResult()
    {
        for (int i = 0; i < howManyDice; i++)
        {
            if(updatedRolledResultSprite[i] == QuestionMarkSprite  || diceButton[i].interactable == false)  // if the sprite is ?, random the int and save it to the result array
            {
                rolledResult[i] = UnityEngine.Random.Range(1,howManyFacePerDice+1);
                Debug.Log(rolledResult[i]);
            }
            else
            {
                continue;
            }
        }

        for (int i = 0; i < howManyFacePerDice; i++) //as a directory, counting each faces show how many times
        {
            eachDiceFaceShownTimes[i] = rolledResult.Count(n => n == i+1);
        }
        
        
    }

    public void ChangeEachDicesSprite()
    {
        for (int i = 0; i < howManyDice; i++) //first time pressing roll button, change all the dice sprite to the array result sprite and save the result sprite to another Sprite array
        {
          updatedRolledResultSprite[i] = originalRolledResultSprite[i] = diceButton[i].GetComponent<Image>().sprite = diceFacesSprite[rolledResult[i]-1];
        }


    }

    public void ForcePlayerNotClickTheScoreButton(bool diceButtonsTOrF,bool rollButtonTOrF)
    {
        rollButton.interactable = rollButtonTOrF;
        rollButton.gameObject.GetComponentInChildren<TMP_Text>().enabled = rollButtonTOrF; 
        foreach (Button diceButton in diceButton){diceButton.interactable = diceButtonsTOrF;} 
    }

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//Dice faces onclick functions
    public void pressed1stDice()
    {
        ChangeDiceFaceToQMOrResult(0);
    }
    public void pressed2ndDice()
    {
        ChangeDiceFaceToQMOrResult(1);
    }
    public void pressed3rdDice()
    {
        ChangeDiceFaceToQMOrResult(2);
    }
    public void pressed4thDice()
    {
        ChangeDiceFaceToQMOrResult(3);
    }
    public void pressed5thDice()
    {
        ChangeDiceFaceToQMOrResult(4);
    }



    public void ChangeDiceFaceToQMOrResult(int index) //when pressed on the dice button, it can change to question mark sprite for reroll that specific dice. if pressed twice, it will return back to the original dice face result
    {
        if (diceButton[index].GetComponent<Image>().sprite != QuestionMarkSprite)
        {
            updatedRolledResultSprite[index] = diceButton[index].GetComponent<Image>().sprite = QuestionMarkSprite;
        }
        else
        {
            updatedRolledResultSprite[index] = diceButton[index].GetComponent<Image>().sprite = originalRolledResultSprite[index];
        }
        
    }

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//condition button onclick functions
    public void OnClickedConditionButton(int index)
    {
        currentPressRollButtonTimes = 0;

        thisConditionButton.GetComponent<Image>().color = darkGreyColor;
        thisConditionButton.GetComponentInChildren<TMP_Text>().color = Color.white;
        
        totalScore = totalScore + eachConditionResult[index];
        totalScoreText.text = totalScore.ToString();

        foreach (Button conditionButton in conditionButton){conditionButton.enabled = false;}
        ForcePlayerNotClickTheScoreButton(false,true);

        timePressedCondition ++;

        if (timePressedCondition == howManyCondition -1)
        {
            ForcePlayerNotClickTheScoreButton(false,false); //TODO move to end page
        }
        
        
    }

    public void pressed1stCondition()
    {
        OnClickedConditionButton(0);
        AddToBonus(0);
    }
    public void pressed2ndCondition()
    {
        OnClickedConditionButton(1);
        AddToBonus(1);
    }
    public void pressed3rdCondition()
    {
        OnClickedConditionButton(2);
        AddToBonus(2);
    }
    public void pressed4thCondition()
    {
        OnClickedConditionButton(3);
        AddToBonus(3);
    }
    public void pressed5thCondition()
    {
        OnClickedConditionButton(4);
        AddToBonus(4);
    }
    public void pressed6thCondition()
    {
        OnClickedConditionButton(5);
        AddToBonus(5);
    }
    public void pressed7thCondition()
    {
        OnClickedConditionButton(6);
    }
    public void pressed8thCondition()
    {
        OnClickedConditionButton(7);
    }
    public void pressed9thCondition()
    {
        OnClickedConditionButton(8);
    }
    public void pressed10thCondition()
    {
        OnClickedConditionButton(9);
    }
    public void pressed11thCondition()
    {
        OnClickedConditionButton(10);
    }
    public void pressed12thCondition()
    {
        OnClickedConditionButton(11);
    }
    public void pressed13thCondition()
    {
        OnClickedConditionButton(12);
    }
    public void pressed14thCondition()
    {
        OnClickedConditionButton(13);
    }
    public void AddToBonus(int index)
    {
        bonusScore += eachConditionResult[index];
        bonusText.text = "("+ bonusScore +")";

        if (bonusScore >= 0 && notHitBonus)
        {
            eachConditionResult[14] = 35;
            totalScore = totalScore + eachConditionResult[14];
            totalScoreText.text = totalScore.ToString();

            bonusText.text = "35";

            notHitBonus = false;
        }
    }

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//change text refer to result

    public void ChangeOnesToSixesConditionText(int index)// Ones to Sixes score calculation method
    {
        conditionButton[index].gameObject.GetComponentInChildren<TMP_Text>().text = (eachDiceFaceShownTimes[index]*(index+1)).ToString();

    }

    public void SavesOnesToSixesScoreToArr(int index)
    {
        eachConditionResult[index] = (eachDiceFaceShownTimes[index]*(index+1));
        
    }

    public void ChangeTextWhichIsFixedResult(int fixedScore, int index) // fullHouse,smallStraight,largeStraight,yahtzee score calculation method
    {
        conditionButton[index].gameObject.GetComponentInChildren<TMP_Text>().text = fixedScore.ToString();
    }

    public void SavesFixedResultToArr(int fixedScore, int index)
    {
        eachConditionResult[index] = fixedScore;
    }
    public void ChangeTextWhichIsSumOfResult(int[] sumOfResult, int index)// 3 OfAKind,4 OfAKind,chance score calculation method
    
    {
        conditionButton[index].gameObject.GetComponentInChildren<TMP_Text>().text = sumOfResult.Sum().ToString();
    }
    public void SavesSumOfResultToArr(int[] sumOfResult, int index)
    {
        eachConditionResult[index] = sumOfResult.Sum();
    }



//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//check result is fit the condition or not;
    
    public void CheckAllConditionFitOrNot()
    {
        isOnesToSixes();
        isThreeOfAKind();
        isFourOfAKind();
        isFullHouse();
        isSmallStraightOrLargeStraight();
        isYahtzee();
        isChance();
    }


    public void isOnesToSixes()
    {
        for (int i = 0; i < howManyFacePerDice; i++)
        {
            if(conditionButton[i].GetComponentInChildren<TMP_Text>().color == Color.white)
            {
                continue;
            }
            else if(eachDiceFaceShownTimes[i] > 0 && conditionButton[i].GetComponentInChildren<TMP_Text>().color != Color.white)
            {
                ChangeOnesToSixesConditionText(i);
                SavesOnesToSixesScoreToArr(i);
            }
            else
            {
                ChangeTextWhichIsFixedResult(0,i);
                SavesFixedResultToArr(0,i);
            }
        }

    }
    public void isThreeOfAKind()
    {
        if(conditionButton[6].GetComponentInChildren<TMP_Text>().color == Color.white)
        {
            return;
        }
        else if(eachDiceFaceShownTimes.Contains(3) || eachDiceFaceShownTimes.Contains(4)||eachDiceFaceShownTimes.Contains(5) && conditionButton[6].GetComponentInChildren<TMP_Text>().color != Color.white)
        {
            ChangeTextWhichIsSumOfResult(rolledResult, 6);
            SavesSumOfResultToArr(rolledResult, 6);
        }
        else
        {
            ChangeTextWhichIsFixedResult(0,6);
            SavesFixedResultToArr(0,6);
        }
        
    }
    public void isFourOfAKind()
    {
        if(conditionButton[7].GetComponentInChildren<TMP_Text>().color == Color.white)
        {
            return;
        }
        else if(eachDiceFaceShownTimes.Contains(4)||eachDiceFaceShownTimes.Contains(5) && conditionButton[7].GetComponentInChildren<TMP_Text>().color != Color.white)
        {
            ChangeTextWhichIsSumOfResult(rolledResult, 7);
            SavesSumOfResultToArr(rolledResult, 7);
        }
        else
        {
            ChangeTextWhichIsFixedResult(0,7);
            SavesFixedResultToArr(0,7);
        }
        
    }
    public void isFullHouse()
    {
        if(conditionButton[8].GetComponentInChildren<TMP_Text>().color == Color.white)
        {
            return;
        }
        else if(eachDiceFaceShownTimes.Contains(3) && eachDiceFaceShownTimes.Contains(2) && conditionButton[8].GetComponentInChildren<TMP_Text>().color != Color.white)
        {
            ChangeTextWhichIsFixedResult(fullHouseScore, 8);
            SavesFixedResultToArr(fullHouseScore,8);
        }
        else
        {
            ChangeTextWhichIsFixedResult(0,8);
            SavesFixedResultToArr(0,8);
        }
    }
    public void isSmallStraightOrLargeStraight()
    {
        for (int j = 9; j < 11; j++)
        {
            if(conditionButton[j].GetComponentInChildren<TMP_Text>().color == Color.white)
            {
                Debug.Log("it is grey color");
                return;
            }
        }
        
        Debug.Log("it is not grey color");

        int currentNumberOfConsecutiveNum = 0;
        int maxNumberOfConsecutiveNum = 0;

        for(int i = 0; i < 6; i++)
        {
            if(eachDiceFaceShownTimes[i] > 0) 
            {
                currentNumberOfConsecutiveNum ++;
                if(maxNumberOfConsecutiveNum < currentNumberOfConsecutiveNum)
                {
                    maxNumberOfConsecutiveNum = currentNumberOfConsecutiveNum;
                }
            }
            else
            {
                currentNumberOfConsecutiveNum = 0;
            }
        }

            
        if(maxNumberOfConsecutiveNum >3 && conditionButton[9].GetComponentInChildren<TMP_Text>().color != Color.white)
        {
            ChangeTextWhichIsFixedResult(smallStraightScore, 9);
            SavesFixedResultToArr(smallStraightScore,6);
        }
        else
        {
            ChangeTextWhichIsFixedResult(0,9);
            SavesFixedResultToArr(0,9);
        }

        if (maxNumberOfConsecutiveNum >4 && conditionButton[10].GetComponentInChildren<TMP_Text>().color != Color.white)
        {
            ChangeTextWhichIsFixedResult(largeStraightScore, 10);
            SavesFixedResultToArr(largeStraightScore,10);
        }
        else
        {
            ChangeTextWhichIsFixedResult(0,10);
            SavesFixedResultToArr(0,10);
        }
        
        
        
    }

    public void isYahtzee()
    {
        
        if(eachDiceFaceShownTimes.Contains(5)&& conditionButton[11].GetComponentInChildren<TMP_Text>().color != Color.white)
        {
            ChangeTextWhichIsFixedResult(YahtzeeScore, 11);
            SavesFixedResultToArr(YahtzeeScore,11);
            timesHitYahtzee++;
        }
        else if (eachDiceFaceShownTimes.Contains(5)&& conditionButton[11].GetComponentInChildren<TMP_Text>().color == Color.white)
        {
            ChangeTextWhichIsFixedResult((YahtzeeScore)+(YahtzeeBonusScore*timesHitYahtzee), 11);
            SavesFixedResultToArr((YahtzeeScore)+(YahtzeeBonusScore*timesHitYahtzee), 11);
            timesHitYahtzee++;
        }
    }

    public void isChance()
    {
        if(conditionButton[12].GetComponentInChildren<TMP_Text>().color == Color.white)
        {
            return;
        }
        else
        {
            SavesSumOfResultToArr(rolledResult, 12);
            ChangeTextWhichIsSumOfResult(rolledResult, 12);
        }
    }











}
