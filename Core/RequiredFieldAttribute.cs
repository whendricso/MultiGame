using UnityEngine;
using System.Collections;
using MultiGame;

namespace MultiGame {

	public class RequiredFieldAttribute : PropertyAttribute {

		public enum RequirementLevels {Optional ,Recommended, Required};
		public RequirementLevels requirementLevel;

		public string helpText;

		public RequiredFieldAttribute (string _helpText, RequirementLevels _requirementLevel) {
			this.requirementLevel = _requirementLevel;
			this.helpText = _helpText;
		}

		public RequiredFieldAttribute (string _helpText) {
			this.requirementLevel = RequirementLevels.Required;
			this.helpText = _helpText;
		}
	}
}