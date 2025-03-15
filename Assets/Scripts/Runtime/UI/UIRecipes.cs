using LoJam.Crafting;
using LoJam.Logic;
using UnityEngine;

namespace LoJam
{
    public class UIRecipes : MonoBehaviour
    {
        [SerializeField] private UIRecipe _uiRecipeL;
        [SerializeField] private UIRecipe _uiRecipeR;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SetRecipeText(Side side, Recipe recipe)
        {
            SetRecipeText(side, recipe.Label);
        }

        public void SetRecipeText(Side side, string text)
        {
            UIRecipe uiRecipe = side == Side.Left ? _uiRecipeL : _uiRecipeR;
            uiRecipe.SetText(text);
        }
    }
}
