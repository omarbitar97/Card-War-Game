Card War Game.

A simple card battle game built with Unity where players draw cards and compete against 
a bot opponent, First to 8 points wins!

Project Overview:

This project started as a technical assignment for a senior unity developer position - build a simple
but complete Unity game that uses real-time API calls. 

How to Play:
1. Click "Draw Card" to begin each round
2. Your card will be drawn from a standard deck
3. The bot opponent will draw their card (with a simulated network delay)
4. Higher card value wins the round (Aces high)
5. First player to reach 8 points wins the game
6. Use the restart button to play again


Code Structure:

The project follows a simple MVC-inspired architecture:

Core Components
1. GameManager: Controls game state, scoring, and win conditions
2. UIManager: Handles all UI updates, card display, and button interactions
3. RoundManager: Manages individual round logic and flow
4. DeckAPIManager: Interfaces with the Deck of Cards API to provide real cards

View Layer
1. UIManager: Handles all visual updates: card images, text, score, and results
2. CardFlipper: Animates card flips when cards are revealed
3. Unity Canvas: UI layout for menus, score display, buttons

Helper Components
1. CardModel: Data model for card information and value calculations
2. ICardService: Interface for deck operations allowing for easy swapping of implementations
3. MainMenuManager: Handles menu navigation and game start/quit functionality
4. LocalDeckService: Fully offline fallback using 52-card local deck and resources
5. GameSceneUIManager: Manages in-game UI functionality like restarting or returning to menu


Technical Implementation

I ran into a few challenges during development that led to some interesting solutions:

1. Async API Integration.
I initially used coroutines for all API calls, but switched to UniTask for better async/await support.
This cleaned up the code significantly and made error handling more straightforward. You can still
see some commented-out coroutine code that I left in for reference.

2. Simulated Network Latency.
To make the bot opponent feel more realistic, I added a randomized "thinking time" of 600-1500ms.
This makes it feel less like you're just playing against a simple algorithm and more like you're
waiting for an actual opponent's move.

3. Separation of Concerns.
I deliberately separated the card service implementation (DeckAPIManager) behind an interface
(ICardService). This would allow easy swapping between the real API and a mock version for
testing, or potentially a local deck implementation if needed.

4. Additional Features.
Beyond the core requirements, I added:

A. Proper error handling for API failures and network issues.

B. Visual feedback for the player during opponent's "thinking" time.

C. Round result display with clear win/loss indication.

D. Game flow management to prevent interaction during card draws.

E. Scene navigation for moving between menu and game.

F. Flip Card Couritine Driven Animatiom

G. Fully offline fallback using 52-card local deck and resources

Testing & Debug Tools
1. Force offline mode via Inspector toggle (GameManager → Force Offline)
2. Debug logs show deck source: API or Local
3. Fallback sprite support for missing card art
4. Scene-safe animations (avoids flipping destroyed objects)

Getting Started
Prerequisites
Unity 2021.3 LTS or newer
Internet connection (required for API calls)

Installation
1. Clone this repository
2. Open the project in Unity
3. Open the MainMenu scene and hit Play

Future Improvements
If I had more time, I'd love to:

1. Add sound effects and more visual polish
2. Implement card animations (flip, deal, etc.)
3. Create a local fallback deck when no internet is available
4. Add different game modes (like blackjack or poker
5. scoring) Improve UI responsiveness on different screen
sizes
6. Add player profiles and win tracking
7. Implement unit tests, especially for the game logic

Acknowledgments:

Deck of Cards API for providing the card data and images UniTask
for making async operations in Unity much cleaner.
