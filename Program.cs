using System;
using System.Collections;
using System.Device.Gpio;
using System.Threading;

namespace SimonSays
{
            // var player1GreenLedPin = 23;
            // var player1YellowLedPin = 22;
            // var player1RedLedPin = 02;
            // var player1BlueLedPin = 04;
            // var player1WhiteLedPin = 21;
            // var player2WhiteLedPin = 19;
            // var player2BlueLedPin = 18;
            // var player2RedLedPin = 05;
            // var player2YellowLedPin = 17;
            // var player2GreenLedPin = 16;

            // var player1GreenButtonPin = 14;
            // var player1YellowButtonPin = 27;
            // var player1RedButtonPin = 26;
            // var player1BlueButtonPin = 25;
            // var player1WhiteButtonPin = 33;
            // var player2WhiteButtonPin = 32;
            // var player2BlueButtonPin = 35;
            // var player2RedButtonPin = 34;
            // var player2YellowButtonPin = 36;
            // var player2GreenButtonPin = 39;

    public class Program
    {
        private static readonly int[] buttonPins = { 14, 27, 26, 25, 33, 32, 35, 34, 36, 39 };
        private static readonly int[] ledPins    = { 23, 22, 02, 04, 21, 19, 18, 05, 17, 16 };

        static GpioPin[] buttons = new GpioPin[10];
        static GpioPin[] leds = new GpioPin[10];
        static ArrayList pattern = new();
        static int patternLength = 0;

        static int triesLeft = 3;

        public static void Main()
        {
            var gpio = new GpioController();

            for (var i = 0; i < 10; i++)
            {
                buttons[i] = gpio.OpenPin(buttonPins[i]);
                buttons[i].SetPinMode(PinMode.Input);
                leds[i] = gpio.OpenPin(ledPins[i]);
                leds[i].SetPinMode(PinMode.Output);
            }

            GeneratePattern();
            
            while (true)
            {
                PlayPattern();
                var success = GetPlayerInput();
                if (success)
                {
                    pattern.Add(new Random().Next(9)+1);
                    patternLength++;
                    BlinkSuccess();
                }
                else
                {
                    triesLeft--;
                    if (triesLeft > 0)
                    {
                        BlinkError();
                        PlayPattern();
                        success = GetPlayerInput();
                        if (!success)
                        {
                            triesLeft--;
                        }
                    }
                    else
                    {
                        BlinkError();
                        pattern.Clear();
                        triesLeft = 3;
                        GeneratePattern();
                    }
                }
                Thread.Sleep(1000);
            }
        }

        static void GeneratePattern()
        {
            pattern.Clear();
            patternLength = 0;
            for (int i = 0; i < 3; i++)
            {
                pattern.Add(new Random().Next(9)+1);
                patternLength++;
            }
            
            Thread.Sleep(1000);
        }

        static void PlayPattern()
        {
            for (int i=0; i<patternLength; i++)
            {
                var j = (int)pattern[i];
                leds[j].Write(PinValue.High);
                Thread.Sleep(1000);
                leds[j].Write(PinValue.Low);
                Thread.Sleep(1000);
            }
        }

        static bool GetPlayerInput()
        {
            for (int i=0; i<patternLength; i++)
            {
                var j = (int)pattern[i];

                var correctInput = false;
                while (!correctInput)
                {
                    for (var x = 0; x < 10; x++)
                    {
                        if (buttons[x].Read() == PinValue.High)
                        {
                            correctInput = true;
                            if (j != x)
                            {
                                return false;
                            }
                            leds[x].Write(PinValue.High);
                            Thread.Sleep(500);
                            leds[x].Write(PinValue.Low);
                        }
                    }
                }
            }
            return true;
        }

        static void BlinkError()
        {
            for (var i = 0; i < 3; i++)
            {
                foreach (var led in leds)
                {
                    led.Write(PinValue.High);
                }
                Thread.Sleep(500);
                foreach (var led in leds)
                {
                    led.Write(PinValue.Low);
                }
                Thread.Sleep(500);
            }
        }

        static void BlinkSuccess()
        {
            foreach (var led in leds)
            {
                led.Write(PinValue.High);
            }
            Thread.Sleep(1000);
            foreach (var led in leds)
            {
                led.Write(PinValue.Low);
            }
            Thread.Sleep(1000);
        }
    }
}
