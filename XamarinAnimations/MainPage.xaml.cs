/*  No Warranty: THE SUBJECT SOFTWARE IS PROVIDED "AS IS" WITHOUT ANY WARRANTY OF ANY KIND, etc.
 *  Author: Tianggee Team :)
 *  
 *  Please download and rate our app 5 stars https://tianggee.com/ :) Help your fellow dev :)
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace XamarinAnimations
{
    public partial class MainPage : ContentPage
    {
        private const uint FramesPerSecond = 60;
        private const uint FrameRate = 1000 / FramesPerSecond;
        private const uint TotalAnimationDurationInMs = 500; //1000 = 1s        
        private const int WaitBeforeExecutingOtherAnimation = 500; //1s

        private double _selectedYPosition;
        private double _selectedXPosition;
        private bool _animationSafelyLoaded = true;

        public MainPage()
        {
            InitializeComponent();
            SetCoffeeSource();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RightToLeftAnimation();
        }

        private void SetCoffeeSource()
        {
            if (!(FindByName("CoffeeList") is StackLayout coffeeListStackLayout))
                return;

            BindableLayout.SetItemsSource(coffeeListStackLayout, GetMyCoffeeList());
        }

        private void RightToLeftAnimation()
        {
            if (!(FindByName("CoffeeList") is StackLayout coffeeListStackLayout))
                return;

            var parentAnimation = new Animation();
            var control = coffeeListStackLayout;

            foreach (var item in control.Children)
            {
                if (!(item is Grid grid))
                    return;

                if (!(grid.Children.FirstOrDefault(i => i is Frame) is Frame frame))
                    return;

                var labels = grid.Children.Where(i => i is Label);
                var enumerable = labels.ToList();

                if (enumerable.Count() < 3)
                    return;

                if (!(enumerable.First() is Label labelWebsite)
                    || !(enumerable.Skip(1).Take(1).FirstOrDefault() is Label labelName)
                    || !(enumerable.LastOrDefault() is Label labelPrice))
                    return;

                var movingLabelWebsiteToTheLeftAnimation = new Animation(d => { labelWebsite.TranslationX = d; },
                    labelWebsite.TranslationX, 0, Easing.Linear);
                var movingLabelNameToTheLeftAnimation = new Animation(d => { labelName.TranslationX = d; },
                    labelName.TranslationX, 0, Easing.Linear);
                var movingLabelPriceToTheLeftAnimation =
                    new Animation(d => { labelPrice.TranslationX = d; }, labelPrice.TranslationX, 0);
                var showingImageAnimation = new Animation(d => frame.Opacity = d, 0, 1);

                parentAnimation.Add(0.0, 1.0, movingLabelWebsiteToTheLeftAnimation);
                parentAnimation.Add(0.0, 1.0, movingLabelNameToTheLeftAnimation);
                parentAnimation.Add(0.0, 1.0, movingLabelPriceToTheLeftAnimation);
                parentAnimation.Add(0.9, 1.0, showingImageAnimation);
            }

            parentAnimation.Commit(control, nameof(RightToLeftAnimation), FrameRate, TotalAnimationDurationInMs,
                Easing.Linear,
                (d, b) =>
                {
                    //do nothing
                });
        }

        private static IEnumerable<Coffee> GetMyCoffeeList()
        {
            return new List<Coffee>
            {
                new Coffee
                {
                    Image = "black.jpg",
                    Details = "just coffee",
                    Name = "Black",
                    Price = "Php 169",
                    TranslationXCol1 = 50,
                    TranslationXCol2 = 150,
                    IsShowImage = false,
                    Description =
                        "is simply coffee that is normally brewed without the addition of additives such as sugar, milk, cream or added flavours"
                },
                new Coffee
                {
                    Image = "latte.jpg",
                    Details = "expresso, steamed milk",
                    Name = "Cafe Latte",
                    Price = "Php 170",
                    TranslationXCol1 = 100,
                    TranslationXCol2 = 200,
                    IsShowImage = false,
                    Description =
                        "Caffe latte is a coffee drink made with espresso and steamed milk. The word comes from the Italian caffè e latte, caffelatte or caffellatte"
                },
                new Coffee
                {
                    Image = "expresso.jpg",
                    Details = "ice expresso",
                    Name = "Expresso",
                    Price = "Php 130",
                    TranslationXCol1 = 200,
                    TranslationXCol2 = 300,
                    IsShowImage = false,
                    Description =
                        "Espresso is a coffee-making method of Italian origin, in which a small amount of nearly boiling water is forced under pressure through finely-ground coffee beans. Espresso coffee can be made with a wide variety of coffee beans and roast levels"
                },
                new Coffee
                {
                    Image = "capucino.jpg",
                    Details = "express steamed milk, foam",
                    Name = "Capucino",
                    Price = "Php 180",
                    TranslationXCol1 = 300,
                    TranslationXCol2 = 400,
                    IsShowImage = false,
                    Description =
                        "A cappuccino is an espresso-based coffee drink that originated in Italy, and is traditionally prepared with steamed milk foam. Variations of the drink involve the use of cream instead of milk, using non-dairy milks, and flavoring with cinnamon or chocolate powder"
                }
            };
        }

        private async void ReloadAnimation_Clicked(object sender, EventArgs e)
        {
            SetCoffeeSource();
            await Task.Delay(500);
            RightToLeftAnimation();
        }

        private void CoffeeItem_Tapped(object sender, EventArgs e)
        {
            if (!_animationSafelyLoaded)
                return;

            if (!(sender is Frame frame))
                return;

            if (!(frame.BindingContext is Coffee coffee))
                return;

            if (!(FindByName("ItemDetails") is Grid itemDetailsGrid))
                return;

            if (!(FindByName("CoffeeList") is StackLayout coffeeListStackLayout))
                return;

            if (!(FindByName("ItemDetailsImageFrame") is Frame itemDetailsImageFrame))
                return;

            if (!(frame.Content is Image mainImage))
                return;

            SetDetailsToGrid(itemDetailsGrid, coffee, frame);
            AnimateDisplayingOfDetails(itemDetailsGrid, coffeeListStackLayout, mainImage, frame, itemDetailsImageFrame);
        }

        private void SetDetailsToGrid(Grid itemDetailsGrid, Coffee coffee, Frame imageFrame)
        {
            if (!(itemDetailsGrid.FindByName<Frame>("ItemDetailsImageFrame") is Frame frame))
                return;

            if (!(frame.Content is Image image))
                return;

            image.Source = ImageSource.FromFile(coffee.Image);

            if (!(itemDetailsGrid.FindByName<Label>("ItemDetailsName") is Label labelName))
                return;

            _selectedYPosition = GetElementAbsoluteYPosition(imageFrame);
            _selectedXPosition = GetElementAbsoluteXPosition(imageFrame);
            labelName.Text = coffee.Name;

            if (!(itemDetailsGrid.FindByName<Label>("ItemDetailsDescription") is Label labelDescription))
                return;

            labelDescription.Text = coffee.Description;

            if (!(itemDetailsGrid.FindByName<Label>("Details") is Label details))
                return;

            details.Text = $"{coffee.Details} for only {coffee.Price}";
        }

        private double GetElementAbsoluteYPosition(View view)
        {
            var y = view.Y;
            var parent = view.Parent as View;

            while (parent != null)
            {
                y += parent.Y;
                parent = parent.Parent as View;
            }

            return y;
        }

        private double GetElementAbsoluteXPosition(View view)
        {
            var x = view.X;
            var parent = view.Parent as View;

            while (parent != null)
            {
                x += parent.X;
                parent = parent.Parent as View;
            }

            return x;
        }

        private void AnimateDisplayingOfDetails(Grid itemDetailsGrid, StackLayout coffeeListStackLayout,
            Image mainImage, Frame imageFrame,
            Frame itemDetailsImageFrame)
        {
            if (!(FindByName("ItemDetailsName") is Label itemDetailsName))
                return;

            if (!(FindByName("Details") is Label details))
                return;

            if (!(FindByName("ItemDetailsDescription") is Label itemDetailsDescription))
                return;

            if (!(FindByName("LabelHeart") is Label labelHeart))
                return;

            _animationSafelyLoaded = false;
            itemDetailsGrid.IsVisible = true;
            var parentAnimation = new Animation();

            const double opacityEnd = 10000000.0;
            var opacityItemDetailsAnimation = new Animation(d =>
            {
                var opacity = d / opacityEnd;
                itemDetailsGrid.Opacity = opacity;
            }, 0, opacityEnd, Easing.Linear);

            var moveItemsUpAnimation = new Animation(d =>
            {
                itemDetailsName.TranslationY = d;
                details.TranslationY = d;
                itemDetailsDescription.TranslationY = d;
                labelHeart.TranslationY = d;
            }, 80, 0, Easing.Linear);

            const int offset = 120;
            var imageAndFrameScaleAnimation =
                new Animation(d => itemDetailsImageFrame.Scale = d, 0.08, 1, Easing.Linear);
            var frameCornerRadiusAnimation =
                new Animation(d => itemDetailsImageFrame.CornerRadius = (float) d, 80, 0, Easing.Linear);
            var imageTranslationXAnimation = new Animation(d => itemDetailsImageFrame.TranslationX = d,
                _selectedXPosition - offset, 0,
                Easing.Linear);
            var imageTranslationYAnimation = new Animation(d => itemDetailsImageFrame.TranslationY = d,
                _selectedYPosition - offset, 0,
                Easing.Linear);

            parentAnimation.Add(0.0, 0.5, moveItemsUpAnimation);
            parentAnimation.Add(0.0, 0.5, imageAndFrameScaleAnimation);
            parentAnimation.Add(0.0, 0.5, frameCornerRadiusAnimation);
            parentAnimation.Add(0.0, 0.5, imageTranslationXAnimation);
            parentAnimation.Add(0.0, 0.5, imageTranslationYAnimation);
            parentAnimation.Add(0.0, 1.0, opacityItemDetailsAnimation);
            parentAnimation.Commit(this, nameof(AnimateDisplayingOfDetails), FrameRate, TotalAnimationDurationInMs,
                Easing.Linear,
                (d, b) =>
                {
                    imageFrame.TranslationX = 0;
                    imageFrame.Scale = 1;
                    mainImage.Scale = 1;
                    coffeeListStackLayout.IsVisible = false;

                    Task.Run(async () =>
                    {
                        await Task.Delay(WaitBeforeExecutingOtherAnimation);
                        _animationSafelyLoaded = true;
                    });
                });
        }

        private void BackArrow_Tapped(object sender, EventArgs e)
        {
            if (!_animationSafelyLoaded)
                return;

            if (!(FindByName("ItemDetails") is Grid itemDetailsGrid))
                return;

            if (!(FindByName("CoffeeList") is StackLayout coffeeListStackLayout))
                return;

            if (!(sender is ContentView contentView))
                return;

            if (!(FindByName("LabelHeart") is Label labelHeart))
                return;

            var detailsImage = (contentView.Parent as Grid)?.FindByName<Frame>("ItemDetailsImageFrame");

            if (detailsImage == null)
                return;

            AnimateHidingOfDetails(itemDetailsGrid, coffeeListStackLayout, detailsImage, labelHeart);
        }

        private void AnimateHidingOfDetails(Grid itemDetailsGrid, StackLayout coffeeListStackLayout,
            Frame detailsImageFrame, Label labelHeart)
        {
            if (!(FindByName("ItemDetailsName") is Label itemDetailsName))
                return;

            if (!(FindByName("Details") is Label details))
                return;

            if (!(FindByName("ItemDetailsDescription") is Label itemDetailsDescription))
                return;

            _animationSafelyLoaded = false;
            var parentAnimation = new Animation();
            var halfWidthMinusOffset = (detailsImageFrame.Width / 2) - 40;

            const double opacityEnd = 1000000.0;
            var opacityItemDetailsAnimation = new Animation(d =>
            {
                var opacity = d / opacityEnd;
                itemDetailsGrid.Opacity = opacity;
            }, opacityEnd, 0, Easing.Linear);

            var coffeeListVisibleAnimation = new Animation(d =>
            {
                if (coffeeListStackLayout.IsVisible) return;
                coffeeListStackLayout.IsVisible = true;
            }, 0, 1, Easing.Linear);

            var detailsImageScaleAnimation = new Animation(d =>
            {
                detailsImageFrame.Scale = d;
                Console.WriteLine($"Alfon scale {d}");
            }, 1, 0.06, Easing.Linear);

            var moveItemsDownAnimation = new Animation(d =>
            {
                itemDetailsName.TranslationY = d;
                details.TranslationY = d;
                itemDetailsDescription.TranslationY = d;
                labelHeart.TranslationY = d;
            }, 0, 80, Easing.Linear);

            var detailsImageTranslationYAnimation = new Animation(d =>
            {
                double minusOffset = 140;
                detailsImageFrame.TranslationY = d - minusOffset;
            }, 0, _selectedYPosition, Easing.SpringOut);

            var detailsImageTranslationXAnimation = new Animation(
                d => detailsImageFrame.TranslationX = d - halfWidthMinusOffset, 0, _selectedXPosition,
                Easing.SpringOut);
            var frameCornerRadiusAnimation =
                new Animation(d => detailsImageFrame.CornerRadius = (float) d, 0, 80, Easing.Linear);

            parentAnimation.Add(0.0, 1.0, moveItemsDownAnimation);
            parentAnimation.Add(0.0, 1.0, frameCornerRadiusAnimation);
            parentAnimation.Add(0.0, 1.0, detailsImageTranslationYAnimation);
            parentAnimation.Add(0.0, 1.0, detailsImageTranslationXAnimation);
            parentAnimation.Add(0.0, 1.0, opacityItemDetailsAnimation);
            parentAnimation.Add(0.0, 1.0, detailsImageScaleAnimation);
            parentAnimation.Add(0.9, 1.0, coffeeListVisibleAnimation);
            parentAnimation.Commit(this, nameof(AnimateHidingOfDetails), FrameRate, TotalAnimationDurationInMs,
                Easing.Linear,
                (d, b) =>
                {
                    labelHeart.TextColor = Color.FromHex("#c6c6c5");
                    detailsImageFrame.Scale = 1;
                    detailsImageFrame.TranslationY = 0;
                    detailsImageFrame.TranslationX = 0;
                    itemDetailsGrid.IsVisible = false;

                    Task.Run(async () =>
                    {
                        await Task.Delay(WaitBeforeExecutingOtherAnimation);
                        _animationSafelyLoaded = true;
                    });
                });
        }

        private void Heart_Tapped(object sender, EventArgs e)
        {
            if (!(FindByName("CoffeeList") is StackLayout coffeeListStackLayout))
                return;

            if (!(sender is Grid grid))
                return;

            if (!(grid.Children.LastOrDefault() is Label labelHeart))
                return;

            if (!(FindByName("Ellipse") is Ellipse ellipse))
                return;

            const string heartRedColor = "#FE251B";
            const string heartGrayColor = "#c6c6c5";
            coffeeListStackLayout.IsVisible = false;
            AnimateHeart(labelHeart, heartRedColor, heartGrayColor, ellipse);
        }

        private void AnimateHeart(Label heart, string heartRedColor, string heartGrayColor, Ellipse ellipse)
        {
            var parentAnimation = new Animation();
            const double startScale = 1.0;
            const double endScale = 1.5;

            var colorHeartScaleUpAnimation = new Animation(d => { heart.Scale = d; }, startScale, endScale,
                Easing.SpringOut, () =>
                {
                    heart.TextColor = (Color.FromHex(heartRedColor) == heart.TextColor)
                        ? Color.FromHex(heartGrayColor)
                        : Color.FromHex(heartRedColor);
                });
            var colorHeartScaleDownAnimation =
                new Animation(d => { heart.Scale = d; }, endScale, startScale, Easing.SpringOut);


            var ellipseFadeIn = new Animation(d => { ellipse.Opacity = d; }, 0, 0.5, Easing.SpringOut);

            var ellipseFadeOut = new Animation(d => { ellipse.Opacity = d; }, 0.5, 0, Easing.SpringOut);

            parentAnimation.Add(0.0, 0.3, ellipseFadeIn);
            parentAnimation.Add(0.3, 0.5, ellipseFadeOut);
            parentAnimation.Add(0.0, 0.5, colorHeartScaleUpAnimation);
            parentAnimation.Add(0.5, 1.0, colorHeartScaleDownAnimation);

            parentAnimation.Commit(this, nameof(AnimateHeart), FrameRate, 900, Easing.SpringOut,
                (d, b) =>
                {
                    //do nothing
                });
        }
    }
}