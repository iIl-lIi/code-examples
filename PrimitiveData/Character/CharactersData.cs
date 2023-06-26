using System;

namespace Data.Character
{
    [Serializable] public class CharactersArray
    {
        public CharacterData[] Characters;
    }
    
    [Serializable] public class CharactersData
    {
        public CharactersArray CharactersArray;
    }
}