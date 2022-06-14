using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public enum eGameState
{
    Ready = 0,
    SecondReady,
    FinalReady,
    Battle,
    SecondBattle,
    FinalBattle,
    Result
}

public class BattleCardWar : MonoBehaviour
{
    public TrackingObject obj_cat_;
    public TrackingObject obj_dog_;

    public eGameState game_state_ = eGameState.Ready;

    public string system_msg_ = "";
    public string bet_msg_ = "";
    public string rule1 = "사용자는 강아지와 고양이 카드를 나누어  가지고, 각 동물의 능력을 확인합니다.\n" +
        "선택한 카드를 카메라에 비추고, 게임시작 버튼을 누르면 게임이 시작됩니다.\n" +
        "배팅은 3번 이루어지며 기본 배팅은 10코인 입니다.\n" +
        "승부는 2개 주사위 합으로 승부를 합니다.\n" +
        "자신의 주사위 합은 cat,dog 버튼을 누르면 확인 가능합니다.\n" +
        "(배팅은 0,10,15,20 으로 정해두었습니다.)\n" +
        "주사위합이 높은 사람이 승리하게 되며,승자는 자신의 최종 배팅만큼 코인을 회복합니다.\n" +
        "(100이상 회복하지 않습니다.)\n" +
        "패자의 경우 승자의 최종 배팅만큼 감소하게  됩니다.\n" +
        "3번의 배팅이 끝나고 코인이 더 많은 사용자가 승리합니다.";

    int status = 0, status2 = 0, status3 = 0, status4 = 0, status5 = 0, status6 = 0, status7 = 0, status8 = 0, status9 = 0, final = 0;
    int stage = 1;

    int cat_coin = 0, dog_coin = 0;
    int dog_ready = 0, cat_ready = 0;
    int dog_bet = 0, cat_bet = 0;
    int sum_dog = 0;
    double sum_cat = 0.0;

    int last_cat_dice = 0;
    int last_dog_dice = 0;

    // 효과음
    public AudioClip audio_dice;
    public AudioClip audio_button1;
    public AudioClip audio_button2;
    public AudioClip audio_coin;
    public AudioClip audio_cat;
    public AudioClip audio_dog;
    public AudioClip audio_congraturation;
    AudioSource audioSource;

    void Awake()
    {
        this.audioSource = GetComponent<AudioSource>();
    }

    /****** 효과음 삽입 구간 ******/
    void PlaySound(string action)
    {
        switch (action)
        {
            case "Dice":
                audioSource.clip = audio_dice;
                break;
            case "Button1":
                audioSource.clip = audio_button1;
                break;
            case "Button2":
                audioSource.clip = audio_button2;
                break;
            case "Coin":
                audioSource.clip = audio_coin;
                break;
            case "Cat":
                audioSource.clip = audio_cat;
                break;
            case "Dog":
                audioSource.clip = audio_dog;
                break;
            case "Congraturation":
                audioSource.clip = audio_congraturation;
                break;
        }
        audioSource.Play();
    }

    void OnGUI()
    {
        GUIStyle gui_style = new GUIStyle();
        gui_style.fontSize = 60;
        gui_style.normal.textColor = Color.cyan;

        GUIStyle gui_style2 = new GUIStyle("Box");
        gui_style2.fontSize = 40;
        gui_style2.normal.textColor = Color.white;

        GUIStyle gui_style_btn = new GUIStyle("Button");
        gui_style_btn.fontSize = 50;

        GUIStyle gui_style_btn2 = new GUIStyle("Button");
        gui_style_btn2.fontSize = 25;

        if (obj_cat_.isDetected == false || obj_dog_.isDetected == false)
        {
            system_msg_ = "카드를 인식시켜주세요.";
            GUI.Label(new Rect(700, 500, 200, 100), system_msg_, gui_style);
        }

        if (obj_cat_.isDetected && obj_dog_.isDetected && game_state_ == eGameState.Result)
        {
            
            if (obj_cat_.coin_ > obj_dog_.coin_)
            {
                
                GUI.Label(new Rect(700, 700, 500, 200), "축하합니다\n고양이가 이겼습니다!!!", gui_style);
                
            }
            if (obj_cat_.coin_ < obj_dog_.coin_)
            {
                GUI.Label(new Rect(700, 700, 500, 200), "축하합니다\n강아지가 이겼습니다!!!", gui_style);
                
            }

            if (GUI.Button(new Rect(600, 500, 350, 150), "재시작", gui_style_btn))
            {
                PlaySound("Button1");
                obj_cat_.coin_ = 100;
                obj_dog_.coin_ = 110;
                game_state_ = eGameState.Ready;

                obj_cat_.obj_text_mesh_.text = obj_cat_.name_ + "\n COIN: " + obj_cat_.coin_;
                obj_dog_.obj_text_mesh_.text = obj_dog_.name_ + "\n COIN: " + obj_dog_.coin_;
            }
        }

        if (obj_cat_.isDetected && obj_dog_.isDetected && game_state_ == eGameState.Ready)
        {
            GUI.Label(new Rect(400, 150, 200, 60), "ROUND 1", gui_style);
            if (GUI.Button(new Rect(600, 500, 350, 150), "Game Start", gui_style_btn))
                
            {
                PlaySound("Dice");
                do
                {
                    last_cat_dice = 1 + Random.Range(0, 6);
                    last_dog_dice = 1 + Random.Range(0, 6);

                    obj_cat_.obj_text_mesh_.text = "주사위 : " + last_cat_dice;
                    obj_dog_.obj_text_mesh_.text = "주사위 : " + last_dog_dice;
                }
                while (last_cat_dice == last_dog_dice);

                obj_cat_.num1 = 1 + Random.Range(0, 6);
                obj_cat_.num2 = 1 + Random.Range(0, 6);
                sum_cat = (double)obj_cat_.num1 + (double)obj_cat_.num2 + 1.5;

                obj_dog_.num1 = 1 + Random.Range(0, 6);
                obj_dog_.num2 = 1 + Random.Range(0, 6);
                sum_dog = obj_dog_.num1 + obj_dog_.num2;

                game_state_ = eGameState.Battle;
            }
        }
        if (obj_cat_.isDetected && obj_dog_.isDetected && game_state_ == eGameState.SecondReady)
        {
            GUI.Label(new Rect(400, 150, 200, 60), "ROUND 2", gui_style);
            if (GUI.Button(new Rect(600, 500, 350, 150), "Round Start", gui_style_btn))
            {
                PlaySound("Dice");
                do
                {
                    last_cat_dice = 1 + Random.Range(0, 6);
                    last_dog_dice = 1 + Random.Range(0, 6);

                    obj_cat_.obj_text_mesh_.text = "주사위 : " + last_cat_dice;
                    obj_dog_.obj_text_mesh_.text = "주사위 : " + last_dog_dice;
                }
                while (last_cat_dice == last_dog_dice);

                obj_cat_.num1 = 1 + Random.Range(0, 6);
                obj_cat_.num2 = 1 + Random.Range(0, 6);
                sum_cat = (double)obj_cat_.num1 + (double)obj_cat_.num2 + 1.5;

                obj_dog_.num1 = 1 + Random.Range(0, 6);
                obj_dog_.num2 = 1 + Random.Range(0, 6);
                sum_dog = obj_dog_.num1 + obj_dog_.num2;

                game_state_ = eGameState.SecondBattle;
            }
        }
        if (obj_cat_.isDetected && obj_dog_.isDetected && game_state_ == eGameState.FinalReady)
        {
            GUI.Label(new Rect(400, 150, 200, 60), "ROUND 3", gui_style);
            if (GUI.Button(new Rect(600, 500, 350, 150), "Round Start", gui_style_btn))
            {
                PlaySound("Dice");
                do
                {
                    last_cat_dice = 1 + Random.Range(0, 6);
                    last_dog_dice = 1 + Random.Range(0, 6);

                    obj_cat_.obj_text_mesh_.text = "주사위 : " + last_cat_dice;
                    obj_dog_.obj_text_mesh_.text = "주사위 : " + last_dog_dice;
                }
                while (last_cat_dice == last_dog_dice);

                obj_cat_.num1 = 1 + Random.Range(0, 6);
                obj_cat_.num2 = 1 + Random.Range(0, 6);
                sum_cat = (double)obj_cat_.num1 + (double)obj_cat_.num2 + 1.5;

                obj_dog_.num1 = 1 + Random.Range(0, 6);
                obj_dog_.num2 = 1 + Random.Range(0, 6);
                sum_dog = obj_dog_.num1 + obj_dog_.num2;

                game_state_ = eGameState.FinalBattle;
            }
        }

        //Battle Start
        if (obj_cat_.isDetected && obj_dog_.isDetected && game_state_ == eGameState.Battle)
        {
            stage = 1;
            //도움말 Start
            if (status == 0)
            {
                if (GUI.Button(new Rect(10, 10, 80, 80), "?", gui_style_btn2))
                {
                    PlaySound("Button1");
                    status = 1;
                }
            }
            if (status == 1)
            {
                GUI.Box(new Rect(660, 100, 1500, 550), "", gui_style2);
                GUI.Box(new Rect(660, 100, 1500, 550), "", gui_style2);
                GUI.Box(new Rect(660, 100, 1500, 550), rule1, gui_style2);

                if (GUI.Button(new Rect(660, 600, 1500, 50), "닫기", gui_style_btn2))
                {
                    PlaySound("Button1");
                    status = 0;
                }
            }
            //도움말 End

            GUI.Label(new Rect(400, 150, 200, 60), "ROUND 1", gui_style);

            //카드숫자 표시 Start
            if (status6 == 0)
            {
                if (GUI.Button(new Rect(300, 500, 80, 80), "Cat", gui_style_btn2))
                {
                    PlaySound("Cat");
                    status6 = 1;
                }
            }
            if (status6 == 1)
            {
                if (GUI.Button(new Rect(300, 600, 80, 80), obj_cat_.num1.ToString(), gui_style_btn2))
                {
                    status6 = 0;
                }
                if (GUI.Button(new Rect(390, 600, 80, 80), obj_cat_.num2.ToString(), gui_style_btn2))
                {
                    status6 = 0;
                }
            }
            if (status7 == 0)
            {
                if (GUI.Button(new Rect(1700, 500, 80, 80), "Dog", gui_style_btn2))
                {
                    PlaySound("Dog");
                    status7 = 1;
                }
            }
            if (status7 == 1)
            {
                if (GUI.Button(new Rect(1700, 600, 80, 80), obj_dog_.num1.ToString(), gui_style_btn2))
                {
                    status7 = 0;
                }
                if (GUI.Button(new Rect(1790, 600, 80, 80), obj_dog_.num2.ToString(), gui_style_btn2))
                {
                    status7 = 0;
                }
            }
            //카드숫자 표시 End

            //배팅 Start
            if (status2 == 0)
            {
                if (GUI.Button(new Rect(460, 730, 200, 120), stage.ToString() + "번째 Stage", gui_style_btn2))
                {
                    PlaySound("Button1");
                    status2 = 1;
                }
            }
            if (status2 == 1)
            {
                //cat 배팅 
                if (GUI.Button(new Rect(150, 850, 200, 120), "0", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_cat_.bet = 0;
                    //system_msg_ = "Dog: 10 배팅했습니다.";
                    cat_ready = 1;
                }
                if (GUI.Button(new Rect(360, 850, 200, 120), "10", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_cat_.bet = 10;
                    //system_msg_ = "Dog: 20 배팅했습니다.";
                    cat_ready = 1;
                }
                if (GUI.Button(new Rect(570, 850, 200, 120), "15", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_cat_.bet = 15;
                    //system_msg_ = "Dog: 30 배팅했습니다.";
                    cat_ready = 1;
                }
                if (GUI.Button(new Rect(780, 850, 200, 120), "20", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_cat_.bet = 20;
                    //system_msg_ = "Dog: 30 배팅했습니다.";
                    cat_ready = 1;
                }
                GUI.Label(new Rect(100, 100, 100, 100), "Cat: " + obj_cat_.bet, gui_style);
            }
            if (status3 == 0)
            {
                if (GUI.Button(new Rect(1500, 720, 200, 120), stage.ToString() + "번째 Stage", gui_style_btn2))
                {
                    PlaySound("Button1");
                    status3 = 1;
                }
            }
            if (status3 == 1)
            {
                // Dog 배팅 오른쪽
                if (GUI.Button(new Rect(1390, 850, 200, 120), "0", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_dog_.bet = 0;
                    //system_msg_ = "Cat: 10 배팅했습니다.";                        
                    dog_ready = 1;
                }
                if (GUI.Button(new Rect(1600, 850, 200, 120), "10", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_dog_.bet = 10;
                    //system_msg_ = "Cat: 10 배팅했습니다.";                        
                    dog_ready = 1;
                }
                if (GUI.Button(new Rect(1810, 850, 200, 120), "15", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_dog_.bet = 15;
                    //system_msg_ = "Cat: 20 배팅했습니다.";                        
                    dog_ready = 1;
                }
                if (GUI.Button(new Rect(2020, 850, 200, 120), "20", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_dog_.bet = 20;
                    //system_msg_ = "Cat: 30 배팅했습니다.";                        
                    dog_ready = 1;
                }
                GUI.Label(new Rect(2100, 100, 100, 100), "Dog: " + obj_dog_.bet, gui_style);
            }
            //배팅 End

            //카드 합 비교 Start
            if (cat_ready == 1 && dog_ready == 1)
            {
                //고양이가 이겼을 때
                if (sum_cat > sum_dog)
                {
                    cat_coin = obj_cat_.coin_;
                    dog_coin = obj_dog_.coin_;

                    cat_bet = obj_cat_.bet;
                    dog_bet = obj_dog_.bet;

                    //체력이 100일때
                    if (cat_coin + obj_cat_.bet + 10 >= 100)
                    {
                        dog_coin = dog_coin - obj_cat_.bet - 10;
                        cat_coin = 100;
                    }
                    else if (cat_coin != 100)
                    {
                        dog_coin = dog_coin - obj_cat_.bet - 10;
                        cat_coin = cat_coin + obj_cat_.bet + 10;
                    }
                    obj_cat_.obj_text_mesh_.text = obj_cat_.name_ + "\n COIN: " + cat_coin;
                    obj_dog_.obj_text_mesh_.text = obj_dog_.name_ + "\n COIN: " + dog_coin;

                    obj_cat_.coin_ = cat_coin;
                    obj_dog_.coin_ = dog_coin;

                    obj_dog_.bet = 0;
                    obj_cat_.bet = 0;
                    game_state_ = eGameState.SecondReady;
                }

                //강아지가 이겼을 때
                if (sum_cat < sum_dog)
                {
                    GUI.Label(new Rect(1500, 100, 100, 100), "결과", gui_style);
                    cat_coin = obj_cat_.coin_;
                    dog_coin = obj_dog_.coin_;

                    cat_bet = obj_cat_.bet;
                    dog_bet = obj_dog_.bet;

                    //체력이 100일때
                    if (dog_coin + obj_dog_.bet + 10 >= 110)
                    {
                        cat_coin = cat_coin - obj_dog_.bet - 10;
                        dog_coin = 110;
                    }
                    else if (dog_coin != 110)
                    {
                        cat_coin = cat_coin - obj_dog_.bet - 10;
                        dog_coin = cat_coin + obj_dog_.bet + 10;
                    }
                    obj_cat_.obj_text_mesh_.text = obj_cat_.name_ + "\n COIN: " + cat_coin;
                    obj_dog_.obj_text_mesh_.text = obj_dog_.name_ + "\n COIN: " + dog_coin;

                    obj_cat_.coin_ = cat_coin;
                    obj_dog_.coin_ = dog_coin;

                    obj_dog_.bet = 0;
                    obj_cat_.bet = 0;
                    game_state_ = eGameState.SecondReady;

                }
            }
            //카드합 비교 End          
        }
        //Battle End

        //SecondBattle Start
        if (obj_cat_.isDetected && obj_dog_.isDetected && game_state_ == eGameState.SecondBattle)
        {
            stage = 2;
            //도움말 Start

            if (status == 0)
            {
                if (GUI.Button(new Rect(10, 10, 80, 80), "?", gui_style_btn2))
                {
                    PlaySound("Button1");
                    status = 1;
                }
            }
            if (status == 1)
            {
                GUI.Box(new Rect(660, 100, 1500, 550), "", gui_style2);
                GUI.Box(new Rect(660, 100, 1500, 550), "", gui_style2);
                GUI.Box(new Rect(660, 100, 1500, 550), rule1, gui_style2);

                if (GUI.Button(new Rect(660, 600, 1500, 50), "닫기", gui_style_btn2))
                {
                    PlaySound("Button1");
                    status = 0;
                }
            }
            //도움말 End

            GUI.Label(new Rect(400, 150, 200, 60), "ROUND 2", gui_style);

            //카드숫자 표시 Start
            if (status8 == 0)
            {
                if (GUI.Button(new Rect(300, 500, 80, 80), "Cat", gui_style_btn2))
                {
                    PlaySound("Cat");
                    status8 = 1;
                }
            }
            if (status8 == 1)
            {
                if (GUI.Button(new Rect(300, 600, 80, 80), obj_cat_.num1.ToString(), gui_style_btn2))
                {
                    status8 = 0;
                }
                if (GUI.Button(new Rect(390, 600, 80, 80), obj_cat_.num2.ToString(), gui_style_btn2))
                {
                    status8 = 0;
                }
            }
            if (status9 == 0)
            {
                if (GUI.Button(new Rect(1700, 500, 80, 80), "Dog", gui_style_btn2))
                {
                    PlaySound("Dog");
                    status9 = 1;
                }
            }
            if (status9 == 1)
            {
                if (GUI.Button(new Rect(1700, 600, 80, 80), obj_dog_.num1.ToString(), gui_style_btn2))
                {
                    status9 = 0;
                }
                if (GUI.Button(new Rect(1790, 600, 80, 80), obj_dog_.num2.ToString(), gui_style_btn2))
                {
                    status9 = 0;
                }
            }
            //카드숫자 표시 End

            //배팅 Start
            if (status4 == 0)
            {
                if (GUI.Button(new Rect(460, 730, 200, 120), stage.ToString() + "번째 Stage", gui_style_btn2))
                {
                    PlaySound("Button1");
                    status4 = 1;
                }
            }
            if (status4 == 1)
            {
                //cat 배팅 
                if (GUI.Button(new Rect(150, 850, 200, 120), "0", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_cat_.bet = 0;
                    //system_msg_ = "Dog: 10 배팅했습니다.";
                    cat_ready = 2;
                }
                if (GUI.Button(new Rect(360, 850, 200, 120), "10", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_cat_.bet = 10;
                    //system_msg_ = "Dog: 20 배팅했습니다.";
                    cat_ready = 2;
                }
                if (GUI.Button(new Rect(570, 850, 200, 120), "15", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_cat_.bet = 15;
                    //system_msg_ = "Dog: 30 배팅했습니다.";
                    cat_ready = 2;
                }
                if (GUI.Button(new Rect(780, 850, 200, 120), "20", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_cat_.bet = 20;
                    //system_msg_ = "Dog: 30 배팅했습니다.";
                    cat_ready = 2;
                }
                GUI.Label(new Rect(100, 100, 100, 100), "Cat: " + obj_cat_.bet, gui_style);
            }
            if (status5 == 0)
            {
                if (GUI.Button(new Rect(1500, 720, 200, 120), stage.ToString() + "번째 Stage", gui_style_btn2))
                {
                    PlaySound("Button1");
                    status5 = 1;
                }
            }
            if (status5 == 1)
            {
                // Dog 배팅 오른쪽
                if (GUI.Button(new Rect(1390, 850, 200, 120), "0", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_dog_.bet = 0;
                    //system_msg_ = "Cat: 10 배팅했습니다.";                        
                    dog_ready = 2;
                }
                if (GUI.Button(new Rect(1600, 850, 200, 120), "10", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_dog_.bet = 10;
                    //system_msg_ = "Cat: 10 배팅했습니다.";                        
                    dog_ready = 2;
                }
                if (GUI.Button(new Rect(1810, 850, 200, 120), "15", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_dog_.bet = 15;
                    //system_msg_ = "Cat: 20 배팅했습니다.";                        
                    dog_ready = 2;
                }
                if (GUI.Button(new Rect(2020, 850, 200, 120), "20", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_dog_.bet = 20;
                    //system_msg_ = "Cat: 30 배팅했습니다.";                        
                    dog_ready = 2;
                }
                GUI.Label(new Rect(2100, 100, 100, 100), "Dog: " + obj_dog_.bet, gui_style);
            }
            //배팅 End

            //카드 합 비교 Start
            if (cat_ready == 2 && dog_ready == 2)
            {

                //고양이가 이겼을 때
                if (sum_cat > sum_dog)
                {
                    cat_coin = obj_cat_.coin_;
                    dog_coin = obj_dog_.coin_;

                    cat_bet = obj_cat_.bet;
                    dog_bet = obj_dog_.bet;

                    //체력이 100일때
                    if (cat_coin + obj_cat_.bet + 10 >= 100)
                    {
                        dog_coin = dog_coin - obj_cat_.bet - 10;
                        cat_coin = 100;
                    }
                    else if (cat_coin != 100)
                    {
                        dog_coin = dog_coin - obj_cat_.bet - 10;
                        cat_coin = cat_coin + obj_cat_.bet + 10;
                    }
                    obj_cat_.obj_text_mesh_.text = obj_cat_.name_ + "\n COIN: " + cat_coin;
                    obj_dog_.obj_text_mesh_.text = obj_dog_.name_ + "\n COIN: " + dog_coin;

                    obj_cat_.coin_ = cat_coin;
                    obj_dog_.coin_ = dog_coin;

                    obj_dog_.bet = 0;
                    obj_cat_.bet = 0;
                    game_state_ = eGameState.FinalReady;
                }

                //강아지가 이겼을 때
                if (sum_cat < sum_dog)
                {

                    cat_coin = obj_cat_.coin_;
                    dog_coin = obj_dog_.coin_;

                    cat_bet = obj_cat_.bet;
                    dog_bet = obj_dog_.bet;

                    //체력이 100일때
                    if (dog_coin + obj_dog_.bet + 10 >= 110)
                    {
                        cat_coin = cat_coin - obj_dog_.bet - 10;
                        dog_coin = 110;
                    }
                    else if (dog_coin != 110)
                    {
                        cat_coin = cat_coin - obj_dog_.bet - 10;
                        dog_coin = cat_coin + obj_dog_.bet + 10;
                    }
                    obj_cat_.obj_text_mesh_.text = obj_cat_.name_ + "\n COIN: " + cat_coin;
                    obj_dog_.obj_text_mesh_.text = obj_dog_.name_ + "\n COIN: " + dog_coin;

                    obj_cat_.coin_ = cat_coin;
                    obj_dog_.coin_ = dog_coin;

                    obj_dog_.bet = 0;
                    obj_cat_.bet = 0;
                    game_state_ = eGameState.FinalReady;
                }
            }
            //카드합 비교 End 
        }
        //SecondBattle End

        //FinalBattle Start
        if (obj_cat_.isDetected && obj_dog_.isDetected && game_state_ == eGameState.FinalBattle)
        {
            stage = 3;
            //도움말 Start

            if (status == 0)
            {
                if (GUI.Button(new Rect(10, 10, 80, 80), "?", gui_style_btn2))
                {
                    PlaySound("Button1");
                    status = 1;
                }
            }
            if (status == 1)
            {
                GUI.Box(new Rect(660, 100, 1500, 550), "", gui_style2);
                GUI.Box(new Rect(660, 100, 1500, 550), "", gui_style2);
                GUI.Box(new Rect(660, 100, 1500, 550), rule1, gui_style2);

                if (GUI.Button(new Rect(660, 600, 1500, 50), "닫기", gui_style_btn2))
                {
                    PlaySound("Button1");
                    status = 0;
                }
            }
            //도움말 End

            GUI.Label(new Rect(400, 150, 200, 60), "FINAL ROUND", gui_style);

            //카드숫자 표시 Start
            if (final == 0)
            {
                if (GUI.Button(new Rect(300, 500, 80, 80), "Cat", gui_style_btn2))
                {
                    PlaySound("Cat");
                    final = 1;
                }
            }
            if (final == 1)
            {
                if (GUI.Button(new Rect(300, 600, 80, 80), obj_cat_.num1.ToString(), gui_style_btn2))
                {
                    final = 0;
                }
                if (GUI.Button(new Rect(390, 600, 80, 80), obj_cat_.num2.ToString(), gui_style_btn2))
                {
                    final = 0;
                }
            }
            if (status9 == 0)
            {
                if (GUI.Button(new Rect(1700, 500, 80, 80), "Dog", gui_style_btn2))
                {
                    PlaySound("Dog");
                    status9 = 1;
                }
            }
            if (status9 == 1)
            {
                if (GUI.Button(new Rect(1700, 600, 80, 80), obj_dog_.num1.ToString(), gui_style_btn2))
                {
                    status9 = 0;
                }
                if (GUI.Button(new Rect(1790, 600, 80, 80), obj_dog_.num2.ToString(), gui_style_btn2))
                {
                    status9 = 0;
                }
            }
            //카드숫자 표시 End

            //배팅 Start
            if (status4 == 0)
            {
                if (GUI.Button(new Rect(460, 730, 200, 120), stage.ToString() + "번째 Stage", gui_style_btn2))
                {
                    PlaySound("Button1");
                    status4 = 1;
                }
            }
            if (status4 == 1)
            {
                //cat 배팅 
                if (GUI.Button(new Rect(150, 850, 200, 120), "0", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_cat_.bet = 0;
                    //system_msg_ = "Dog: 10 배팅했습니다.";
                    cat_ready = 3;
                }
                if (GUI.Button(new Rect(360, 850, 200, 120), "10", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_cat_.bet = 10;
                    //system_msg_ = "Dog: 20 배팅했습니다.";
                    cat_ready = 3;
                }
                if (GUI.Button(new Rect(570, 850, 200, 120), "15", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_cat_.bet = 15;
                    //system_msg_ = "Dog: 30 배팅했습니다.";
                    cat_ready = 3;
                }
                if (GUI.Button(new Rect(780, 850, 200, 120), "20", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_cat_.bet = 20;
                    //system_msg_ = "Dog: 30 배팅했습니다.";
                    cat_ready = 3;
                }
                GUI.Label(new Rect(100, 100, 100, 100), "Cat: " + obj_cat_.bet, gui_style);
            }
            if (status5 == 0)
            {
                if (GUI.Button(new Rect(1500, 720, 200, 120), stage.ToString() + "번째 Stage", gui_style_btn2))
                {
                    PlaySound("Button1");
                    status5 = 1;
                }
            }
            if (status5 == 1)
            {
                // Dog 배팅 오른쪽
                if (GUI.Button(new Rect(1390, 850, 200, 120), "0", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_dog_.bet = 0;
                    //system_msg_ = "Cat: 10 배팅했습니다.";                        
                    dog_ready = 3;
                }
                if (GUI.Button(new Rect(1600, 850, 200, 120), "10", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_dog_.bet = 10;
                    //system_msg_ = "Cat: 10 배팅했습니다.";                        
                    dog_ready = 3;
                }
                if (GUI.Button(new Rect(1810, 850, 200, 120), "15", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_dog_.bet = 15;
                    //system_msg_ = "Cat: 20 배팅했습니다.";                        
                    dog_ready = 3;
                }
                if (GUI.Button(new Rect(2020, 850, 200, 120), "20", gui_style_btn))
                {
                    PlaySound("Button2");
                    obj_dog_.bet = 20;
                    //system_msg_ = "Cat: 30 배팅했습니다.";                        
                    dog_ready = 3;
                }
                GUI.Label(new Rect(2100, 100, 100, 100), "Dog: " + obj_dog_.bet, gui_style);
            }
            //배팅 End

            //카드 합 비교 Start
            if (cat_ready == 3 && dog_ready == 3)
            {

                //고양이가 이겼을 때
                if (sum_cat > sum_dog)
                {
                    cat_coin = obj_cat_.coin_;
                    dog_coin = obj_dog_.coin_;

                    cat_bet = obj_cat_.bet;
                    dog_bet = obj_dog_.bet;

                    //체력이 100일때
                    if (cat_coin + obj_cat_.bet + 10 >= 100)
                    {
                        dog_coin = dog_coin - obj_cat_.bet - 10;
                        cat_coin = 100;
                    }
                    else if (cat_coin != 100)
                    {
                        dog_coin = dog_coin - obj_cat_.bet - 10;
                        cat_coin = cat_coin + obj_cat_.bet + 10;
                    }
                    obj_cat_.obj_text_mesh_.text = obj_cat_.name_ + "\n COIN: " + cat_coin;
                    obj_dog_.obj_text_mesh_.text = obj_dog_.name_ + "\n COIN: " + dog_coin;

                    obj_cat_.coin_ = cat_coin;
                    obj_dog_.coin_ = dog_coin;

                    obj_dog_.bet = 0;
                    obj_cat_.bet = 0;
                    PlaySound("Congraturation");
                    game_state_ = eGameState.Result;
                }

                //강아지가 이겼을 때
                if (sum_cat < sum_dog)
                {

                    cat_coin = obj_cat_.coin_;
                    dog_coin = obj_dog_.coin_;

                    cat_bet = obj_cat_.bet;
                    dog_bet = obj_dog_.bet;

                    //체력이 100일때
                    if (dog_coin + obj_dog_.bet + 10 >= 110)
                    {
                        cat_coin = cat_coin - obj_dog_.bet - 10;
                        dog_coin = 110;
                    }
                    else if (dog_coin != 110)
                    {
                        cat_coin = cat_coin - obj_dog_.bet - 10;
                        dog_coin = cat_coin + obj_dog_.bet + 10;
                    }
                    obj_cat_.obj_text_mesh_.text = obj_cat_.name_ + "\n COIN: " + cat_coin;
                    obj_dog_.obj_text_mesh_.text = obj_dog_.name_ + "\n COIN: " + dog_coin;

                    obj_cat_.coin_ = cat_coin;
                    obj_dog_.coin_ = dog_coin;

                    obj_dog_.bet = 0;
                    obj_cat_.bet = 0;

                    PlaySound("Congraturation");
                    game_state_ = eGameState.Result;
                }
            }
            //카드합 비교 End 
        }
        //SecondBattle End
    }
}