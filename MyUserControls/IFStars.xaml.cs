using ItemFinder_WPF.MyPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ItemFinder_WPF.MyUserControls
{
    /// <summary>
    /// Logique d'interaction pour IFStars.xaml
    /// </summary>
    /// 
    enum StarState
    {
        Empty,
        Filled,
        HalfFilled
    }

    public partial class IFStars : UserControl
    {
        IDictionary<Path, StarState> starsStates;
        int maxfill = 0;

        public IFStars()
        {
            InitializeComponent();
            DataContext = this;
            
        }

        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable",
                                                                                                        typeof(bool),
                                                                                                        typeof(IFIconButton));
        public bool Editable
        {
            get { return (bool)GetValue(EditableProperty); }
            set { SetValue(EditableProperty, value); }

        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
                                                                                                        typeof(double),
        
                                                                                                        typeof(IFIconButton));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }

        }


        private void Star1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(!Editable) { return; }
            Border currentBorder = (Border)sender;
            Path currentStar = (Path)currentBorder.Child;
            StarState currentStarState = starsStates[currentStar];
            int currentStarIndex = starsStates.Keys.ToList().IndexOf(currentStar);
            double internalValue = (currentStarIndex + 1) - 0.5;

            if(currentStarState == StarState.Empty || (currentStarState == StarState.Filled && maxfill != currentStarIndex))
            {
                //On met les étoiles remplies jusqu'à celle cliquée
                for (int i = 0; i <= currentStarIndex-1; i++)
                {
                    Path currentStarLoop = starsStates.ElementAt(i).Key;
                    currentStarLoop.Data = (Geometry)Application.Current.TryFindResource("StarFilled");
                    starsStates[currentStarLoop] = StarState.Filled;
                }
                currentStar.Data = (Geometry)Application.Current.TryFindResource("StarHalfFilled");
                starsStates[currentStar] = StarState.HalfFilled;
                //On met les étoiles vides de celle cliquée à la fin
                if (currentStarIndex != 4)
                {
                    for (int i = currentStarIndex + 1; i <= 4; i++)
                    {
                        Path currentStarLoop = starsStates.ElementAt(i).Key;
                        currentStarLoop.Data = (Geometry)Application.Current.TryFindResource("StarUnfilled");
                        starsStates[currentStarLoop] = StarState.Empty;
                    }
                }

                maxfill = currentStarIndex;
            }
            else if(currentStarState == StarState.Filled && maxfill == currentStarIndex)
            {
                currentStar.Data = (Geometry)Application.Current.TryFindResource("StarHalfFilled");
                starsStates[currentStar] = StarState.HalfFilled;
            }
            else
            {
                currentStar.Data = (Geometry)Application.Current.TryFindResource("StarFilled");
                starsStates[currentStar] = StarState.Filled;
                internalValue += 0.5;
            }
            Value = internalValue;
        }

        private void updateValue()
        {
            if (Value % 1 == 0)
            {
                //Pas de demi étoile
                for (int i = 0; i <= Convert.ToInt32(Value) - 1; i++)
                {
                    Path currentStarLoop = starsStates.ElementAt(i).Key;
                    currentStarLoop.Data = (Geometry)Application.Current.TryFindResource("StarFilled");
                    starsStates[currentStarLoop] = StarState.Filled;
                }
            }
            else
            {
                //Demi étoile présente
                for (int i = 0; i < Convert.ToInt32(Value) - 1; i++)
                {
                    Path currentStarLoop = starsStates.ElementAt(i).Key;
                    currentStarLoop.Data = (Geometry)Application.Current.TryFindResource("StarFilled");
                    starsStates[currentStarLoop] = StarState.Filled;
                }
                int currentIndex = Convert.ToInt32(Value) - 1;
                if(currentIndex < 0) { currentIndex = 0; }
                Path halfFilled = starsStates.ElementAt(currentIndex).Key;
                halfFilled.Data = (Geometry)Application.Current.TryFindResource("StarHalfFilled");
                starsStates[halfFilled] = StarState.HalfFilled;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        { 
            starsStates = new Dictionary<Path, StarState>() {
                {Star1, StarState.Empty },
                {Star2, StarState.Empty },
                {Star3, StarState.Empty },
                {Star4, StarState.Empty },
                {Star5, StarState.Empty }
            };

            updateValue();
        }

        private void Border_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(!Editable) { return; }
            Border currentBorder = (Border)sender;
            Path currentStar = (Path)currentBorder.Child;
            StarState currentStarState = starsStates[currentStar];
            int currentStarIndex = starsStates.Keys.ToList().IndexOf(currentStar);
            if(currentStarState == StarState.HalfFilled)
            {
                currentStar.Data = (Geometry)Application.Current.TryFindResource("StarUnfilled");
                starsStates[currentStar] = StarState.Empty;
                Value = Value - 0.5;
            }
        }
    }
}
