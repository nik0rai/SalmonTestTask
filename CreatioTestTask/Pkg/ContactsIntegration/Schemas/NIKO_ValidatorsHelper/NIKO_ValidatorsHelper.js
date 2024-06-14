 define("NIKO_ValidatorsHelper", ["NIKO_ConstValues"], function(NIKO_ConstValues) {
	return {
		/* Валидация почты, где 
		 * оба поля не должны быть пусты.
		 * @param {String} email Поле почты.
		 * @param {String} phone Поле телефона.
		 * @return {{invalidMessage: String}} 
         * Объект с текстом ошибки.
		 */
		emailValidator: function(email, phone) {
			let message = "";      

			if(email) {
				if(!email.match(NIKO_ConstValues.RegExPatterns.GmailPattern)) {
					message = "Ivalid email (see example: someone@gmail.com)";
				}	
			}
            if (Ext.isEmpty(email) && Ext.isEmpty(phone)) {
            	message = "Phone or Email field shouldn't be empty"
            }
						
            return {
            	invalidMessage: message
            };
		},
		/* Валидация телефона, где 
	     * оба поля не должны быть пусты.
		 * @param {String} email Поле почты.
		 * @param {String} phone Поле телефона.
		 * @return {{invalidMessage: String}} 
         * Объект с текстом ошибки.
		 */
		phoneValidator: function(email, phone) {
			let message = "";
	
			if(phone) {
				if(!phone.match(NIKO_ConstValues.RegExPatterns.PhonePattern)) {
					message = "Ivalid phone number (should start with + and contain 9 numbers)";
				}	
			}
            if (Ext.isEmpty(email) && Ext.isEmpty(phone)) {
            	message = "Phone or Email field shouldn't be empty"
            }
				
            return {
            	invalidMessage: message
            };
		},
	};
});