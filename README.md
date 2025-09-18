# Raw Deal Card Game - IIC2113 Project

## Project Description

This project is a C# implementation of the **Raw Deal** collectible card game, developed as part of the IIC2113 Detailed Software Design course at Pontificia Universidad Católica de Chile.

The game simulates a wrestling match between two superstars, where each player uses a 60-card deck along with a superstar card. The main objective is to reduce the opponent's arsenal to zero cards by using maneuvers, actions, and reversals.

## Key Features

### Game Elements
- **Superstars**: Each player selects a superstar with unique abilities and specific attributes (Hand size, Superstar Value, Superstar Ability)
- **Cards**: Maneuver (yellow), Action (light blue), Reversal (red), and Hybrid (two colors)
- **Card attributes**: Title, Type, Subtype, Fortitude (F), Damage (D), Stun Value (SV), and special effects
- **Game zones**: Superstar, Arsenal, Ringside, Ring Area, and Hand

### Implemented Rules
- ✅ Deck validation (60 cards, copy limits, heel/face restrictions, superstar-exclusive cards)
- ✅ Turn mechanics (draw segment, main segment, turn ends)
- ✅ Damage and fortitude rating system
- ✅ Reversals from hand and from arsenal
- ✅ Special card effects and superstar abilities
- ✅ Victory conditions (Pin Victory and Count Out Victory)

## Supported Superstars

The game includes the 7 original superstars with their unique abilities:

| Superstar | Hand Size | Superstar Value | Ability |
|-----------|-----------|-----------------|---------|
| **Stone Cold** | ? | ? | Once during your turn, draw a card but place one from hand to bottom of Arsenal |
| **The Undertaker** | ? | ? | Discard 2 cards to take 1 card from Ringside to hand |
| **Mankind** | ? | ? | Always draw 2 cards; opponent's damage at -1D |
| **HHH** | 10 | ? | No special ability |
| **The Rock** | 5 | ? | Start of turn: move 1 card from Ringside to bottom of Arsenal |
| **Kane** | ? | ? | Start of turn: opponent moves top Arsenal card to Ringside |
| **Chris Jericho** | ? | ? | Discard a card to force opponent to discard a card |

## Card Effect Types Implemented

- 🃏 Basic damage cards (no effects)
- 🗑️ Card discarding mechanics
- ⚡ Damage (D) and fortitude (F) modification
- 🛡️ Irreversible maneuvers
- 📚 Card drawing effects
- 🔄 Special reversals
- 🔍 Searching in arsenal/ringside
- 💥 Collateral damage
- ♻️ Damage recovery
- ✨ Various unique effects


## Requirements

- .NET 6.0 or later
- Visual Studio 2022 or compatible IDE

## Installation & Running

```bash
# Clone the repository
git clone <repository-url>

# Navigate to project directory
cd RawDealGame

# Build the solution
dotnet build

# Run the game
dotnet run --project RawDealGame

```

## How to Play

- **Deck Building:** Create a 60-card deck following the validation rules.  
- **Superstar Selection:** Choose one of the 7 available superstars.  
- **Game Setup:** Shuffle decks and draw initial hand based on superstar's hand size.  
- **Turns:** Players alternate turns consisting of:
  - **Draw Phase:** Draw cards up to hand size.  
  - **Main Phase:** Play maneuvers, actions, and reversals.  
  - **Turn End:** Resolve end-of-turn effects and pass to opponent.  
- **Victory:** Reduce your opponent's arsenal to zero cards to win.

---

## Development Highlights

### OOP Principles Applied
- **Inheritance:** Base card classes with specialized implementations.  
- **Polymorphism:** Unified interface for card effects and abilities.  
- **Encapsulation:** Proper data hiding and method exposure.  
- **SOLID Compliance:** Single responsibility, open/closed principles applied throughout the project.

### Design Patterns Used
- **Strategy Pattern:** For card effects and superstar abilities.  
- **State Pattern:** To manage game states efficiently.  
- **Factory Pattern:** For creating cards and superstar instances.  
- **Observer Pattern:** To handle game event notifications and updates.

---

## Example Code Structure

```csharp
// Example card implementation
public class HeadButt : ManeuverCard
{
    public HeadButt() : base(
        title: "Head Butt",
        fortitude: 0,
        damage: 4,
        subtypes: new[] { "Strike" })
    {
        AddEffect(new DiscardCardEffect(1));
    }
}
```

## All Documentation and Rules
[Proyecto_Enunciado_General.pdf](https://github.com/user-attachments/files/22400959/Proyecto_Enunciado_General.pdf)

