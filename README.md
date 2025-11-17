# Blasphemous 2 Input System

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



# Features ( las funciones especificas y como funcionan)
