Start Scene File:

	SplashScreen.unity is the starting scene of the game
	Alpha.unity is the starting scene for actual gameplay

How to Play: 

	Use a controller or keyboard (inputs explained in tutorial, but for left stick/WASD 
	control movement, right stick/mouse to look around, RT/shift to run, Controller A or 
	Controller X or keyboard space to jump or interact)

	The objective for this level is to find the three chicks on the map and herd them into the chicken coops near their parents. 

Where to Observe Technology Requirements: 

	3D Character with Real-Time Control
		Cat has well-developed, game-feel like movement and fluid animation. 
			Includes a state machine
			Is able to interact with things in environment
			Controls mapped intuitively
			IK corrections implemented for interacting with certain objects on ground
			Camera follows player
			Some auditory Feedback added

	Gameplay and GameFeel
		Objective based level (herd 3 chickens)
		Player is notified upon completing goal or losing to geese
		Respawn on death implemented
		Goals communicated to player via smoking duck
		Player is able to interact with game world objects and inhabitants
			AI Animals, Chainsaw, Tires, Key
	
	3D world with Physics and Spatial simulation
		Synthesized environment (with use of 3rd party assets) with boundaries which confine the player. 
		

	Interact-able physics simulated world with environment
		Tires, which bounce kitty in the air upon interaction
		A key, which opens a gate to the farm field
		Chicken coops collect chickens and keep track of them.
		Other various objects around which interact when kitty is nearby
			Scarecrow
			Chainsaw

	Real-time NPC Steering Behaviors
		Several animals implemented, several with state machines
		Animations play for some animals on state change (chicks, geese)
		Animals also have appropriate AI 
			Horse, who run to waypoints
			Chickens, who patrol and flee when kitty is too close
			Geese, who patrol and attack kitty when she is close, and flee after
			Cow, who patrols near th garage
			Ducks, who smoke and are all around bad influences
	Polish
		Scenes for main menu, pause menu.
		GUI elements show health and catnip count
		Pause menu
		Splash Screen
		Tutorial which explains controls
		EventManger implemented
			AudioManager, as well.
		Dialogue system implemented
	

Known Problem Areas:
	Menus not working properly with controllers yet. Unity's built-in functionality for that is limited and we’ll probably have to do it via code.
	Chickens might walk into coops on their own

Individual Contributions:

	Geoff 

	Created Kitty’s PlayerController
		PlayerController.cs
	Set up the Player State Machine
		PlayerAttackAction.cs
	PlayerBaseAction.cs
	PlayerHitAction.cs
	PlayerBaseState.cs
	PlayerDieState.cs
	PlayerFallState.cs
	PlayerFreeLookState.cs
	PlayerIdleState.cs
	PlayerInteractState.cs
	PlayerJumpState.cs
	PlayerLieState.cs
	PlayerMeowAction.cs
	PlayerMoveBase.cs
	PlayerMoveState.cs
	PlayerSitState.cs
	PlayerStateMachine.cs

	Built Kitty’s Animation Controller
	⁃	States: Idle, Move, Jump, Fall, Interact Die
	⁃	Actions: Attack, Hit
	⁃	Additive Actions: Meow
	Set up the input controls
	Set up the Cinemachine camera
	Created an “Interactable” component to easily generate trigger collider events. Features a custom EditorGUI with dropdowns.
		InteractionEvent.cs
		Interactable.cs
	Created the RandomInt behavior for passing a random integers into animator states
	RandomStateBehavior.cs
	AnimationStateEvent behavior for alerting the PlayerController or other listeners of key events on animation enter, exit or time.
		AnimationStateEvent.cs
		AnimationStateEventBehavior.cs


James 
	Created the horse
		HorseController.cs
	Created a generalized waypoint-based pathing AI (WIP)
		WaypointAI.cs
	Created AudioManager
		AudioEvent.cs
		AudioManager.cs
	QOL in Unity Editor improvements
		GameObjectExtension.cs
		TagSelectAttributes.cs
		TagSelectorPropertyDrawer.cs


Paul 
	Used existing assets to construct basic level with boundaries
	Created Bouncing Tires Prefab
		Squash/Stretch Animation
		TireController.cs
	Created Chicken Prefab and Chicken Coops to interact with them
		ChickenAI.cs
		CoopController.cs
		CoopGroupController.cs
	Created Goose Prefab
		GooseAI.cs
	Created Key Prefab
		KeyController.cs
		Opens Gate in the level
	Composed original music for Main Menu and GameWorld

Ben
	NPC Cow jumps about excitedly when interacted with. The Cow initially gives an introduction Dialogue, but then randomly selects from 3 follow 	
	up dialogues when the player returns.
	CowFollow.cs
	CowDialogue.asset
	CowFollowupDialogue1.asset
	CowFollowupDialogue2.asset
	CowFollowupDialogue3.asset

Calvin
	Created Dialogue Manager/dialogue system
		DialogueManager.cs
		Dialogue.cs
	DialogueOpenEvent.cs
		DialogueCloseEvent.cs
	Created main menu
		MainMenuController.cs
	Created pause menu
		PauseMenuController.cs
	Created interactive environmental prefabs
		MailboxController.cs
		RockingChairController.cs
		TriggerInteractiveController.cs
	Created inventory & accompanying UI
		PlayerInventory.cs
		PlayerPickupController.cs
	Contributed to in-game audio & scripting
		MusicEvent.cs
		VolumeChangeEvent.cs
	Created splash screen & level transitions
		LevelManager.cs
		SplashScreenController.cs
	Created tutorial and smoking duck with particle system
		TutorialManager.cs
	Created objective system
		Objective.cs

