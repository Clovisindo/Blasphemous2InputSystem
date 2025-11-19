# Blasphemous 2 Input System

# Instructions
Movement: keyboard w/a/s/d, gamepad left joystick.
Jump: keyboard space, gamepad A(xbox), X(playstation).
Attack: keyboard F, gamepad X(xbox), square(playstation).
Dash: keyboard R, gamepad Y(xbox), triangle(playstation).

Player has two textbox that show the current state of move and action state machines, and changes color by the current move state.
Collide with the purple box generate a hurt event and push back the player.
Dash make player invulnerable to damage.
Attack is a dummy action.
When player receive ten hits, trigger the death state and a reboot is necessary.
<img width="613" height="261" alt="imagen" src="https://github.com/user-attachments/assets/b4517826-f17a-4ee9-9360-b13bfb4270c2" />


# Overview
The goal with this project is to create an input system that could work and expand to match the Blasphemous 2 game 's design requirements.
Making an arquitecture with the principles of good design like: open/closed design, interface and layers segretation, single responsability, dependency injection, design driven by domain, automatic test and anothers arquitecture pattern that fit the project

Our first step is to analyze all the system from the game that modify the player domain and which one do so through inputs.

# Blasphemous 2 player domain
The penitent has the following features:
- **Move**: directional move, jump, dash, interact.
- **Skills**: heal, change weapon, prayers, passive skills.
- **Weapons**: normal, heavy and special attack.
- **Game system**: pause, map, inventory, move camera.

To focus in design and future expandable features, we'll focus in the current design:
- An input system that works with keyboard and gamepad, and can change on the fly.
- InputBuffer system for smooth transitions and flow.
- Player controller and state machine that implemente the following states: idle, move, jump, dash, attack, hurt and death.
- Apply the design patterns already mentioned.

# Arquitecture
<img width="459" height="634" alt="imagen" src="https://github.com/user-attachments/assets/4a8f7767-63f5-4598-b9e8-8622f60121fa" />

## Player input
This is the Unity's input action system, class and flow implementation.

## Input Strategy ( keyboard, gamepad and replay)
Interface and different implementations for each input device.
Read input action unity system and generate with command pattern, the  correct command object on a stacked list.

## Input Adapter Service
Detect and change the actual input device(InputDeviceWatcher) and publish the method to read de list of inputCommands.

## Player Controller monobehaviour
Read the queue from Input Service and call PlayerApplicationService to process each command and and playerAppService.Update().

## Player Application Service
Runs two state machines: move and action.
Process each command in the state machine that runs that type of command.
Executes current state machines Update() methods.
Check transition rules for the move and action state machines.
Read inputBuffer to check combos ( read but don't have any combo implemented yet).
Implement several events to be called from the different states: player receive attack, when player takes damage or die,etc.
Some of this logic is separated in another service "Player Domain Service" for better layer design.

## Player state machine
Check rule transition on move or action internal level(Example: cant enter in dash state from jump state).
Execute and process the commands receive from the InputService.
Modify the player entity domain based in this commands.

## Player Entity
Save and modify all the player data: position, status, health,etc.
Separate in different components: move, health, damage controller, capabilities.
When any data is changed, send an event to the player View to show these changes.

## Player View (monobehaviour)
The view layer of the flow, show the player and current status.



# Features

## Bootstrapper
Initialize all the components and dependencies injections.
Controls the initialize correct order.
Interface ICoreDependent to initialize objects in scene, after bootstrapper finish.
  It would be better to only initialize from instantiation from bootstrapper and not objects already loaded in scene, but I found it interesting to try this hybrid solution and how it would works.



## Strategy pattern
Provides specific pattern and workflow for each input device.
For example: keyboard and gamepad using unity's input action system generate different information, but we need to normalize this data in order to generate a standard MoveCommand, so that the upper layers don't have to know the specific implementation.

## Layers design
<img width="498" height="310" alt="imagen" src="https://github.com/user-attachments/assets/d44854e2-66f7-442f-8aad-70b620c469a6" />

Benefits of this design:
- Responsability separation.
- Better dependencies order.
- Easy to expand.
- Easy tests.

## Modular Assemblies
For each layer We create a indepent assembly, more easy to avoid circle reference and organize dependencies.

## Programming on DDD
When We design focus on domain, we're more independent for Unity or any engine We use.
This give us lot of advantages from better and more readable design, better prototyping and iteration without depending on the view layer, improved portability, less coupling and more scalability, make unit test or even change your engine.

## Parrarel Finite State Machine (FSM)
In a first version of the state machines, I found that when the player receive damage and block their movement, in some cases We need that the previous state still working, for example receive a hit in the middle of a jump( this error could be avoided with a better previous design and definition the full requirements of the player actions).

Then We proceed to a dual state machine model: one for the movement states and another with the states that need to happen at the same time of some movement actions.
Both state machines run at the same time, and check the rules We define in the analysis before transition to new states.
We use three elements to control this workflow:
Rules for internal use in each type SM: this define the flow between their own states.
Rules for move and action states: when a new action state try to enter, it would verify if its a correct state to the actual move state.
Player entity capabilities: the action states can disable the capabilities of the player like move, jump or receive damage, in order to allow to move actions perform their actual work with the correct conditions.With the previous example, when player jump and is hit, player can't move but the jump state will continuous to calculate the falling.

<img width="616" height="513" alt="image" src="https://github.com/user-attachments/assets/f2c04cce-c622-4477-b079-762fee845cb6" />
<img width="574" height="287" alt="imagen" src="https://github.com/user-attachments/assets/dab7213e-f86e-4e02-a1ad-7c15979a6451" />
<img width="262" height="229" alt="imagen" src="https://github.com/user-attachments/assets/f45b3298-0362-4a75-bf7e-6d2f11aaa3b9" />
<img width="253" height="289" alt="imagen" src="https://github.com/user-attachments/assets/d7b54c8b-7d6d-4669-86f4-feb30ae1c564" />
<img width="257" height="344" alt="imagen" src="https://github.com/user-attachments/assets/ca9efc8f-7402-4fa5-a0ad-e3a59134f50b" />



# Improvements
Add pending functions from the initial analysis: special attacks, combo system, change weapon, different type of damage and resistence, heal, move camera, different types of attacks.





  
