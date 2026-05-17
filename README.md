# CSharp-Unity-Learning-Project

## About The Project
This is a 3D Third-Person Perspective (TPP) project developed primarily as a learning sandbox to improve my C# scripting and Unity Engine skills. Rather than a fully commercial game, this project serves as a practical implementation of core game development mechanics and object-oriented programming concepts.

It was built to demonstrate a solid understanding of physics, input handling, UI management, and data structuring within the Unity ecosystem.

## Key Mechanics & Features Implemented
* **Advanced Player Movement:** Built with Unity's New Input System. Features dynamic jumping physics, gravity handling, and a custom "Shift-Lock" style aiming mechanic (locking character rotation to the camera's forward vector via Right-Click).
* **Raycasting:** Implemented a laser shooting mechanic using physical Raycasts to interact with environmental collectibles and hazardous obstacles.
* **Ragdoll Physics:** A dynamic death state that smoothly disables the Animator and transitions the character's bones into a full physics-based ragdoll upon collision with specific tags.
* **Data Management:** Utilizes `ScriptableObjects` to cleanly store, manage, and scale generic game parameters (e.g., item point values) outside of hardcoded scripts.
* **Dual Camera System:** Features a primary gameplay camera and a secondary top-down Minimap camera that tracks player movement dynamically without rendering over the Canvas UI.
* **UI/UX Architecture:** A structured Canvas system managing score updates, Game Over, and Victory states seamlessly.

## Technologies Used
* **Unity Engine** (Universal Render Pipeline)
* **C#** (Game Logic & Scripting)

## Author
**Hüdalfa Bera Dalgın**
*Applied Computer Science Student*
