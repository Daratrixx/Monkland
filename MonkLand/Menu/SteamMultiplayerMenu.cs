﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu;
using UnityEngine;
using RWCustom;
using System.IO;
using Monkland.SteamManagement;
using Monkland.Patches;

namespace Monkland {
    class SteamMultiplayerMenu: Menu.Menu {

        public MultiplayerChat gameChat;
        public MultiplayerPlayerList playerList;

        public SimpleButton backButton;
        public SimpleButton startGameButton;
        public SimpleButton readyUpButton;
        public HorizontalSlider bodyRed;
        public HorizontalSlider bodyGreen;
        public HorizontalSlider bodyBlue;
        public HorizontalSlider eyesRed;
        public HorizontalSlider eyesGreen;
        public HorizontalSlider eyesBlue;
        public FLabel settingsLabel;
        public FLabel bodyLabel;
        public FLabel eyesLabel;
        public RoundedRect colorBox;

        private FSprite slugcat;
        private FSprite eyes;
        private FSprite darkSprite;
        private FSprite blackFadeSprite;
        private float blackFade;
        private float lastBlackFade;
        private bool gameStarting = false; // boolean variables default to false

        public SteamMultiplayerMenu(ProcessManager manager, bool shouldCreateLobby = false) : base( manager, ProcessManager.ProcessID.MainMenu ) {

            if( shouldCreateLobby ) {
                MonklandSteamManager.instance.CreateLobby();
            }

            #region UI ELEMENTS SIZE DEFINITION
            float resOffset = (1366 - manager.rainWorld.screenSize.x) / 2; // shift everything to the right depending on the resolution. 1/2 of the diff with the max resolution.

            float screenWidth = manager.rainWorld.screenSize.x;
            float screenHeight = manager.rainWorld.screenSize.y;
            // initialize some dynamic sizes and positions for UI elements.
            // player list and multiplayer settings have the same size no matter what the resolution.
            // the room chat adjusts to fill all the middle space
            float panelGutter = 15; // gap width between the panels

            float playerListWidth = 200; // constant
            float multiplayerSettingsWidth = 300; // constant
            float multiplayerSettingsSliderWidth = multiplayerSettingsWidth - 115; // constant
            float roomChatWidth = screenWidth - ( playerListWidth + multiplayerSettingsWidth + panelGutter * 4 ); // depends on resolution

            float playerListPositionX = panelGutter; // position depending on size
            float multiplayerSettingsPositionX = screenWidth - (multiplayerSettingsWidth + panelGutter); // position depending on screen width
            float multiplayerSettingsSliderPositionX = multiplayerSettingsPositionX + 10; // constant
            float roomChatPositionX = playerListWidth + panelGutter * 2; // position depending on player list width

            float controlButtonSpaceHeight = 100; // constant
            float panelHeight = screenHeight - (controlButtonSpaceHeight + panelGutter * 2); // leaving a space for control buttons
            float panelPositionY = controlButtonSpaceHeight; // constant
            #endregion

            this.blackFade = 1f;
            this.lastBlackFade = 1f;
            this.pages.Add( new Page( this, null, "main", 0 ) );
            //this.scene = new InteractiveMenuScene( this, this.pages[0], MenuScene.SceneID.Landscape_SU );
            //this.pages[0].subObjects.Add( this.scene );
            this.darkSprite = new FSprite( "pixel", true );
            this.darkSprite.color = new Color( 0f, 0f, 0f );
            this.darkSprite.scaleX = screenWidth;
            this.darkSprite.scaleY = screenHeight;
            this.darkSprite.x = screenWidth / 2f;
            this.darkSprite.y = screenHeight / 2f;
            this.darkSprite.alpha = 0.85f;
            this.pages[0].Container.AddChild( this.darkSprite );
            this.blackFadeSprite = new FSprite( "Futile_White", true );
            this.blackFadeSprite.scaleX = 87.5f;
            this.gameStarting = false; // redondant
            this.blackFadeSprite.scaleY = 50f;
            this.blackFadeSprite.x = screenWidth / 2f;
            this.blackFadeSprite.y = screenHeight / 2f;
            this.blackFadeSprite.color = new Color( 0f, 0f, 0f );
            Futile.stage.AddChild( this.blackFadeSprite );

            #region MULTIPLAYER SETTINGS
            //Multiplayer Settings Box
            colorBox = new RoundedRect(this, this.pages[0], new Vector2(resOffset + multiplayerSettingsPositionX, panelPositionY), new Vector2(multiplayerSettingsWidth, panelHeight), false);
            this.pages[0].subObjects.Add(colorBox);

            //Settings Label
            settingsLabel = new FLabel("font", "Multiplayer Settings");
            settingsLabel.SetPosition(new Vector2(multiplayerSettingsPositionX + 70, screenHeight - 60));
            Futile.stage.AddChild(this.settingsLabel);

            //Body Color Label
            bodyLabel = new FLabel("font", "Body Color");
            bodyLabel.SetPosition(new Vector2(multiplayerSettingsPositionX + 70, screenHeight - 90));
            Futile.stage.AddChild(this.bodyLabel);

            //Red Slider
            bodyRed = new HorizontalSlider(this, this.pages[0], "Red", new Vector2(resOffset+multiplayerSettingsSliderPositionX, screenHeight - 130), new Vector2(multiplayerSettingsSliderWidth, 30), (Slider.SliderID)patch_Slider.SliderID.BodyRed, false);
            bodyRed.floatValue = 1f;
            bodyRed.buttonBehav.greyedOut = false;
            this.pages[0].subObjects.Add(this.bodyRed);
            //Green Slider
            bodyGreen = new HorizontalSlider(this, this.pages[0], "Green", new Vector2(resOffset+multiplayerSettingsSliderPositionX, screenHeight - 170), new Vector2(multiplayerSettingsSliderWidth, 30), (Slider.SliderID)patch_Slider.SliderID.BodyGreen, false);
            bodyGreen.floatValue = 1f;
            bodyGreen.buttonBehav.greyedOut = false;
            this.pages[0].subObjects.Add(this.bodyGreen);
            //Blue Slider
            bodyBlue = new HorizontalSlider(this, this.pages[0], "Blue", new Vector2(resOffset+multiplayerSettingsSliderPositionX, screenHeight - 210), new Vector2(multiplayerSettingsSliderWidth, 30), (Slider.SliderID)patch_Slider.SliderID.BodyBlue, false);
            bodyBlue.floatValue = 1f;
            bodyBlue.buttonBehav.greyedOut = false;
            this.pages[0].subObjects.Add(this.bodyBlue);

            //Eye Color Label
            eyesLabel = new FLabel("font", "Eye Color");
            eyesLabel.SetPosition(new Vector2(multiplayerSettingsPositionX + 70, screenHeight - 240));
            Futile.stage.AddChild(this.eyesLabel);

            //Red Slider
            eyesRed = new HorizontalSlider(this, this.pages[0], "Red ", new Vector2(resOffset+multiplayerSettingsSliderPositionX, screenHeight - 280), new Vector2(multiplayerSettingsSliderWidth, 30), (Slider.SliderID)patch_Slider.SliderID.EyesRed, false);
            eyesRed.floatValue = 0f;
            this.pages[0].subObjects.Add(this.eyesRed);
            //Green Slider
            eyesGreen = new HorizontalSlider(this, this.pages[0], "Green ", new Vector2(resOffset+multiplayerSettingsSliderPositionX, screenHeight - 320), new Vector2(multiplayerSettingsSliderWidth, 30), (Slider.SliderID)patch_Slider.SliderID.EyesGreen, false);
            eyesGreen.floatValue = 0f;
            this.pages[0].subObjects.Add(this.eyesGreen);
            //Blue Slider
            eyesBlue = new HorizontalSlider(this, this.pages[0], "Blue ", new Vector2(resOffset+multiplayerSettingsSliderPositionX, screenHeight - 360), new Vector2(multiplayerSettingsSliderWidth, 30), (Slider.SliderID)patch_Slider.SliderID.EyesBlue, false);
            eyesBlue.floatValue = 0f;
            this.pages[0].subObjects.Add(this.eyesBlue);
            
            
            //Slugcat Eyes Sprite
            eyes = new FSprite("FoodCircleB", true);
            eyes.scaleX = 1f;
            eyes.scaleY = 1f;
            eyes.color = new Color(0, 0, 0);
            eyes.x = multiplayerSettingsPositionX + 130;
            eyes.y = manager.rainWorld.screenSize.y - 236;
            eyes.isVisible = true;
            this.pages[0].Container.AddChild(this.eyes);

            //Slugcat Sprite
            slugcat = new FSprite("slugcatSleeping", true);
            slugcat.scaleX = 1f;
            slugcat.scaleY = 1f;
            slugcat.color = new Color(1f, 1f, 1f);
            slugcat.x = multiplayerSettingsPositionX + 136;
            slugcat.y = manager.rainWorld.screenSize.y - 235;
            slugcat.isVisible = true;
            this.pages[0].Container.AddChild(this.slugcat);
            #endregion

            #region CONTROL BUTTONS
            //Back button
            this.backButton = new SimpleButton(this, this.pages[0], base.Translate( "BACK" ), "EXIT", new Vector2(resOffset+100f, 50f ), new Vector2( 110f, 30f ));
            this.pages[0].subObjects.Add(this.backButton);

            //Back button
            this.backButton = new SimpleButton( this, this.pages[0], base.Translate( "BACK" ), "EXIT", new Vector2(resOffset+100f, 50f ), new Vector2( 110f, 30f ) );
            this.pages[0].subObjects.Add( this.backButton );
            // => those two lines seem to be redondant

            //Start Game button
            this.startGameButton = new SimpleButton( this, this.pages[0], "Start Game", "STARTGAME", new Vector2(resOffset+screenWidth - 125, 50f ), new Vector2( 110f, 30f ) );
            this.pages[0].subObjects.Add( this.startGameButton );

            //Ready Up button
            this.readyUpButton = new SimpleButton( this, this.pages[0], "Ready UP", "READYUP", new Vector2(resOffset+screenWidth - 250, 50f ), new Vector2( 110f, 30f ) );
            this.pages[0].subObjects.Add( this.readyUpButton );
            #endregion

            //Multiplayer Chat
            this.gameChat = new MultiplayerChat( this, this.pages[0], new Vector2(resOffset+roomChatPositionX, panelPositionY ), new Vector2( roomChatWidth, panelHeight ) );
            this.pages[0].subObjects.Add( this.gameChat );

            //Invite menu
            playerList = new MultiplayerPlayerList( this, this.pages[0], new Vector2(resOffset+playerListPositionX, panelPositionY ), new Vector2( playerListWidth, panelHeight ), new Vector2( playerListWidth - 20, playerListWidth - 20 ) );
            this.pages[0].subObjects.Add( this.playerList );

            //Controller Combatability
            this.bodyRed.nextSelectable[1] = this.readyUpButton;
            this.bodyRed.nextSelectable[3] = this.bodyGreen;
            this.bodyGreen.nextSelectable[1] = this.bodyRed;
            this.bodyGreen.nextSelectable[3] = this.bodyBlue;
            this.bodyBlue.nextSelectable[1] = this.bodyGreen;
            this.bodyBlue.nextSelectable[3] = this.eyesRed;
            this.eyesRed.nextSelectable[1] = this.bodyBlue;
            this.eyesRed.nextSelectable[3] = this.eyesGreen;
            this.eyesGreen.nextSelectable[1] = this.eyesRed;
            this.eyesGreen.nextSelectable[3] = this.eyesBlue;
            this.eyesBlue.nextSelectable[1] = this.eyesGreen;
            this.eyesBlue.nextSelectable[3] = this.readyUpButton;
            this.readyUpButton.nextSelectable[0] = this.backButton;
            this.readyUpButton.nextSelectable[1] = this.eyesBlue;
            this.readyUpButton.nextSelectable[2] = this.startGameButton;
            this.readyUpButton.nextSelectable[3] = this.bodyRed;
            this.startGameButton.nextSelectable[0] = this.readyUpButton;
            this.startGameButton.nextSelectable[1] = this.eyesBlue;
            this.startGameButton.nextSelectable[2] = this.backButton;
            this.startGameButton.nextSelectable[3] = this.bodyRed;
            this.backButton.nextSelectable[0] = this.startGameButton;
            this.backButton.nextSelectable[2] = this.readyUpButton;
            this.backButton.nextSelectable[1] = this.bodyRed;
            this.backButton.nextSelectable[3] = this.eyesBlue;

            //Some Nice Music :)
            if (manager.musicPlayer != null)
            {
                manager.musicPlayer.MenuRequestsSong("NA_05 - Sparkles", 1.2f, 10f);
            }
        }

        protected override void Init() {
            base.Init();
        }

        public override void SliderSetValue(Slider slider, float f)
        {
            if (f < 0.004f)
            {
                f = 0.004f;
            }
            if (MonklandSteamManager.connectedPlayers.Count > 0)
            {
                switch (slider.ID)
                {
                    case ((Slider.SliderID)patch_Slider.SliderID.BodyRed):
                        MonklandSteamManager.GameManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)] = new Color(f, MonklandSteamManager.GameManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].g, MonklandSteamManager.GameManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].b);
                        MonklandSteamManager.GameManager.SendColor(0);
                        return;
                    case ((Slider.SliderID)patch_Slider.SliderID.BodyGreen):
                        MonklandSteamManager.GameManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)] = new Color(MonklandSteamManager.GameManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].r, f, MonklandSteamManager.GameManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].b);
                        MonklandSteamManager.GameManager.SendColor(1);
                        return;
                    case ((Slider.SliderID)patch_Slider.SliderID.BodyBlue):
                        MonklandSteamManager.GameManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)] = new Color(MonklandSteamManager.GameManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].r, MonklandSteamManager.GameManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].g, f);
                        MonklandSteamManager.GameManager.SendColor(2);
                        return;
                    case ((Slider.SliderID)patch_Slider.SliderID.EyesRed):
                        MonklandSteamManager.GameManager.playerEyeColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)] = new Color(f, MonklandSteamManager.GameManager.playerEyeColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].g, MonklandSteamManager.GameManager.playerEyeColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].b);
                        MonklandSteamManager.GameManager.SendColor(3);
                        return;
                    case ((Slider.SliderID)patch_Slider.SliderID.EyesGreen):
                        MonklandSteamManager.GameManager.playerEyeColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)] = new Color(MonklandSteamManager.GameManager.playerEyeColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].r, f, MonklandSteamManager.GameManager.playerEyeColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].b);
                        MonklandSteamManager.GameManager.SendColor(4);
                        return;
                    case ((Slider.SliderID)patch_Slider.SliderID.EyesBlue):
                        MonklandSteamManager.GameManager.playerEyeColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)] = new Color(MonklandSteamManager.GameManager.playerEyeColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].r, MonklandSteamManager.GameManager.playerEyeColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].g, f);
                        MonklandSteamManager.GameManager.SendColor(5);
                        return;
                }
            }
        }

        public override float ValueOfSlider(Slider slider)
        {
            if (MonklandSteamManager.connectedPlayers.Count > 0)
            {
                switch (slider.ID)
                {
                    case ((Slider.SliderID)patch_Slider.SliderID.BodyRed):
                        return MonklandSteamManager.GameManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].r;
                    case ((Slider.SliderID)patch_Slider.SliderID.BodyGreen):
                        return MonklandSteamManager.GameManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].g;
                    case ((Slider.SliderID)patch_Slider.SliderID.BodyBlue):
                        return MonklandSteamManager.GameManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].b;
                    case ((Slider.SliderID)patch_Slider.SliderID.EyesRed):
                        return MonklandSteamManager.GameManager.playerEyeColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].r;
                    case ((Slider.SliderID)patch_Slider.SliderID.EyesGreen):
                        return MonklandSteamManager.GameManager.playerEyeColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].g;
                    case ((Slider.SliderID)patch_Slider.SliderID.EyesBlue):
                        return MonklandSteamManager.GameManager.playerEyeColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].b;

                }
            }
            return 0f;
        }

        public override void Update() {
            this.lastBlackFade = this.blackFade;
            float num = 0f;
            if (this.blackFade < num) {
                this.blackFade = Custom.LerpAndTick(this.blackFade, num, 0.05f, 0.06666667f);
            } else {
                this.blackFade = Custom.LerpAndTick(this.blackFade, num, 0.05f, 0.125f);
            }
            if (MonklandSteamManager.connectedPlayers.Count > 0)
            { 
                slugcat.color = MonklandSteamManager.GameManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)];
                //Debug.Log("Color: " + MonklandSteamManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].r + ", " + MonklandSteamManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].g + ", " + MonklandSteamManager.playerColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)].b);
                eyes.color = MonklandSteamManager.GameManager.playerEyeColors[MonklandSteamManager.connectedPlayers.IndexOf(NetworkGameManager.playerID)];
            }
            if ( NetworkGameManager.managerID == NetworkGameManager.playerID ) {
                //startGameButton.pos = new Vector2( 1060, 50f );
                startGameButton.buttonBehav.greyedOut = gameStarting;
                readyUpButton.buttonBehav.greyedOut = gameStarting;
                backButton.buttonBehav.greyedOut = gameStarting;
            }
            else
            {
                //startGameButton.pos = new Vector2(-100000, 50f);
                startGameButton.buttonBehav.greyedOut = true;
                readyUpButton.buttonBehav.greyedOut = gameStarting;
                backButton.buttonBehav.greyedOut = gameStarting;
            }
            base.Update();
        }

        public override void GrafUpdate(float timeStacker) {
            base.GrafUpdate( timeStacker );
            this.blackFadeSprite.alpha = Mathf.Lerp( this.lastBlackFade, this.blackFade, timeStacker );
        }

        public override void ShutDownProcess() {
            base.ShutDownProcess();
            gameChat.ClearMessages();
            playerList.ClearList();
            this.darkSprite.RemoveFromContainer();
            this.eyes.RemoveFromContainer();
            this.slugcat.RemoveFromContainer();
            this.eyesLabel.RemoveFromContainer();
            this.bodyLabel.RemoveFromContainer();
            this.settingsLabel.RemoveFromContainer();
        }

        public override void Singal(MenuObject sender, string message) {
            if (message == "EXIT") {
                if (manager.musicPlayer != null) { 
                    manager.musicPlayer.FadeOutAllSongs(5f);
                    this.manager.musicPlayer.MenuRequestsSong("RW_8 - Sundown", 1.4f, 2f);
                }
                manager.RequestMainProcessSwitch( ProcessManager.ProcessID.MainMenu );
                MonklandSteamManager.instance.OnGameExit();
                Steamworks.SteamMatchmaking.LeaveLobby( SteamManagement.MonklandSteamManager.lobbyID );
            } else if( message == "READYUP" ) {
                MonklandSteamManager.GameManager.SendColor(0);
                MonklandSteamManager.GameManager.SendColor(1);
                MonklandSteamManager.GameManager.SendColor(2);
                MonklandSteamManager.GameManager.SendColor(3);
                MonklandSteamManager.GameManager.SendColor(4);
                MonklandSteamManager.GameManager.SendColor(5);
                MonklandSteamManager.GameManager.ToggleReady();
            } else if( message == "STARTGAME" ) {
                if (manager.musicPlayer != null)
                {
                    manager.musicPlayer.FadeOutAllSongs(5f);
                }
                if ( NetworkGameManager.isManager) {
                    settingsLabel.text = "";
                    bodyLabel.text = "";
                    eyesLabel.text = "";
                    base.PlaySound(SoundID.MENU_Switch_Page_In);
                    gameStarting = true;
                    MonklandSteamManager.GameManager.QueueStart();
                }
            }
        }
    }
}