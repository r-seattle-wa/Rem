using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;

namespace MemeGenerator
{
    public class MemeTemplateBuilder
    {
        private readonly string _imagePath;
        private readonly List<InputField> _inputFields;
        private string _name;
        private string _description;

        public MemeTemplateBuilder(string imagePath)
        {
            _imagePath = imagePath;
            _inputFields = new List<InputField>();
            _name = "";
            _description = "";
        }

        public MemeTemplateBuilder WithName(string name)
        {
            _name = name ?? throw new ArgumentNullException($"{nameof(name)} cannot be null.");
            return this;
        }

        public MemeTemplateBuilder WithDescription(string description)
        {
            _description = description ?? throw new ArgumentNullException($"{nameof(description)} cannot be null.");
            return this;
        }

        public MemeTemplateBuilder WithInputField(InputField inputField)
        {
            if (inputField == null)
            {
                throw new ArgumentNullException($"{nameof(inputField)} cannot be null.");
            }
            _inputFields.Add(inputField);
            return this;
        }
        
        public MemeTemplate Build()
        {
            var img = Image.Load(_imagePath);
            return new MemeTemplate(_name, _description, img, _inputFields);
        }
    }
}
