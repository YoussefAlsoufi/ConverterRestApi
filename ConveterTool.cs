using ConverterRestApi.Data;
using ConverterRestApi.Migrations;
using ConverterRestApi.Model;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace ConverterRestApi
{
    public class ConveterTools 
    {
        private readonly DataAccess _dataAccess;
        public Dictionary<string,string?> LengthSection { get; private set; }
        public Dictionary<string, string?> DataTypeSection { get; private set; }
        public Dictionary<string, string?> TemperatureSection { get; private set; }

        public ConveterTools(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            var lengthUnitsList = _dataAccess.LengthUnits.ToList();
            var dataUnitsList = _dataAccess.DataUnits.ToList();
            var temperatureUnitsList = _dataAccess.TemperatureUnits.ToList();
            LengthSection = lengthUnitsList.ToDictionary(item => item.UnitName, item => item.Value);
            DataTypeSection = dataUnitsList.ToDictionary(item => item.UnitName, item => item.Value);
            TemperatureSection = temperatureUnitsList.ToDictionary(item => item.UnitName, item => item.Value);
        }


        public Response DoConvert(string num, string fromUnit, string toUnit)
        {
            Response response = new();
            double result;
            string resultMessage;
            var (checkStatus, usedSection) = GenericCheckInputs(num, fromUnit, toUnit);
            
            if (checkStatus)
            {
                double inputValue = Convert.ToDouble(num);
                if (usedSection == TemperatureSection)
                {
                    result = TempConvert(inputValue, Singularize(fromUnit), Singularize(toUnit));
                }
                else
                {

                    double from = Convert.ToDouble(usedSection[Singularize(fromUnit)]);
                    double to = Convert.ToDouble(usedSection[Singularize(toUnit)]);

                    result = inputValue * from / to;

                }

                resultMessage = $"( {num} {fromUnit},  {toUnit}) -> {result.ToString()}";
                response.ResMsg = resultMessage;
                response.ResCode = 200;
                return response;
            }
            else
            {
                resultMessage = $"Please, Check your Inputs again! you have entered incorrect units or nulls.";

                response.ResMsg = resultMessage;
                response.ResCode = 400;
                return response;
            }

        }

        private (bool valid, Dictionary<string,string> UsedSection) GenericCheckInputs(string inputNum, string fromUnit, string toUnit)
        {
            Dictionary<string, string?> usedSection = null;
            bool wrongInputs = true;

            if (LengthSection.ContainsKey(Singularize(fromUnit)))
            {
                //usedSection = LengthSection;
                usedSection = LengthSection;

            }
            else if (DataTypeSection.ContainsKey(Singularize(fromUnit)))
            {
                usedSection = DataTypeSection;
            }
            else if (TemperatureSection.ContainsKey(Singularize(fromUnit)))
            {
                usedSection = TemperatureSection;
            }
            else
            {
                wrongInputs = false;
            }

            bool emptyCheck = (!string.IsNullOrEmpty(inputNum)) && (!string.IsNullOrEmpty(fromUnit)) && (!string.IsNullOrEmpty(toUnit));
            bool validNum = int.TryParse(inputNum, out int n);
            bool validInput = usedSection.ContainsKey(Singularize(fromUnit)) && usedSection.ContainsKey(Singularize(toUnit));
            bool positiveValue = true ? (TemperatureSection.ContainsKey(Singularize(fromUnit)) || n > 0) : false;

            return (validNum && emptyCheck && wrongInputs && positiveValue, usedSection);
        }

        private static double TempConvert(double tempInput, string fromTempUnit, string toTempUnit)
        {
            if (fromTempUnit == "celsiu")
            {
                var result = (tempInput * 1.8) + 32;
                return result;
            }
            return (tempInput - 32) / 1.8;
        }

        protected static string Singularize(string inputString)
        {
            if (!string.IsNullOrEmpty(inputString))
            {
                var Singular = System.Data.Entity.Design.PluralizationServices.PluralizationService.CreateService(new System.Globalization.CultureInfo("en-us"));
                return Singular.Singularize(inputString).ToLower().Trim();
            }
            else
            {
                return "";
            }


        }

    }


}

