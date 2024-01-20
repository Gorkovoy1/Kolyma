using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*this is very much a WIP system and i dont know how perfectly it will work yet. The biggest challenge I havent solved for is the conditional cards ("if your opponent has a yellow class card, swap it out") and the duplicate related cards.
for now, I am catching these with the CONDITIONAL keyword, and will handle that problem later- i'll implement those last, for now they do nothing.
the idea is that a list with a sequence of these keywords will tell the game manager how to execute this card's effects. 
this enum stores info on WHICH EFFECTS and who they TARGET.
SpecialDeckCard.intOne/intTwo/intThree will store the associated numbers such as HOW MANY cards to discard or points to add/subtract.
This data will be read in GameManager and the card's effects will be performed in GameManager.ExecuteCard.
SpecialCards.keywords should be formatted in this order: [EFFECT KEYWORD], [TARGET KEYWORD(S)] [CARD TYPE KEYWORD] (if needed). In other words the first element of the list should always be the EFFECT and the last should be CARD TYPE.
The first time a given effect expects a numerical value, it will check intOne. The next time it needs a numerical value, it will check intTwo. Then, intThree. It will not check more variables than required numerical values. 
in other words, if you only need one number represented, the values of intTwo and intThree do not matter.

for example, a card that says "give yourself -2" will be represented as:
keywords = ADDVALUE, PLAYER
intOne = -2
intTwo = N/A
intThree = N/A

another example, a card that says "you and your opponent both draw a special card"
keywords = DRAW, PLAYER, OPPONENT, SPECIAL
intOne = 1
intTwo = 1
intThree = N/A

another example, a card that says "you draw 1 special and opponent draws 2 specials"
keywords = DRAW, PLAYER, OPPONENT, SPECIAL
intOne = 1
intTwo = 2
intThree = N/A

More functionality will be added in the future to support more complex cards. For example, what if multiple commands could be represented on one card? Example of how the future might look:
a card that says "Give yourself +2. Your opponent discards a special card."
keywords = ADDVALUE, PLAYER, ENDCOMMAND (nyi), DISCARD, OPPONENT, SPECIAL
intOne = 2
intTwo = 1
intThree = N/A

NOTE: I DONT ACTUALLY KNOW IF THIS IS THE BEST WAY TO DO THIS. But i think it might be? the hope is to end up with a system where (at least simple) cards can be added and modified just by editing their object files.
then you wont have to go into code and edit the script every time you want to tune the rules. easier iteration is a game dev's best friend wheeeee!!!
we COULD use regex to decipher the string descriptions of the cards, but this is rough because 1) its definitely slower than doing it this way performancewise 2) it relies on you using the *exact same phrasing system on card descs forever*
maybe limited use of regex to handle the more niche cases like the duplicate and conditional cards. still exploring that. 
either way consider this a science experiment. gonna try this method described above, if it doesnt work i'll learn something and figure out a different way.
*/

public enum SpecialKeyword {
    /*use these keywords for dictating the action to be performed*/
    EFFECT_NONE,
    EFFECT_ADDVALUE,
    EFFECT_DRAW,
    EFFECT_DISCARD,
    EFFECT_CONDITIONAL,

    /*use these keywords for targeting*/
    TARGET_PLAYER,
    TARGET_OPPONENT,

    /*use these keywords for card type*/
    TYPE_NUMBER,
    TYPE_SPECIAL,

    /* used to demarcate the end of an effect in cards that have more than one effect*/
    END_COMMAND,

    /*used to store an integer parameter for a conditional function. Remember, enumerators also correspond to integers*/
    CON_STORE_ADDITIONAL_INTEGER,

    /*use these to call different conditional functions*/
    CON_HAS_CLASS_CARD,
    CON_HAS_VALUE_CARD,
    CON_HAS_DUPLICATE,
    CON_DISCARD_FLAG,
    CON_SWAP_FLAG,
    CON_FLIP_FLAG,
    CON_TRANSFER_FLAG,
    CON_CARD_QUANTITY,
    SUCCESS_PATH,
    FAILURE_PATH,


};

[CreateAssetMenu(fileName = "New Special Card")]
public class SpecialDeckCard : GenericCard
{
    [Header("Card Info")]
    public string description;

    [Header("Keywords for Card Effects/Targets")]
    [Header("EFFECT -> TARGET(S) -> TYPE")]
    public List<SpecialKeyword> keywords = new List<SpecialKeyword>();

    [Header("Numerical Values for Card Effects")]
    public List<int> values;

}
