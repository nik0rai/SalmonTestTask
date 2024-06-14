define("NIKO_ContactAddress_Detail", [], function() {
	return {
		entitySchemaName: "NIKO_ContactAddress",
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		diff: /**SCHEMA_DIFF*/[]/**SCHEMA_DIFF*/,
		methods: {
			/* Сделать филтрацию по NIKO_IsActive. */
			addGridDataColumns: function(esq) {
				this.callParent(arguments);

				var isActive = esq.addColumn("NIKO_IsActive", "NIKO_IsActive");
				isActive.orderPosition = 0;
				isActive.orderDirection = Terrasoft.OrderDirection.DESC;
			},
			
			/* Покрасить в зеленый если NIKO_IsActive = true */
			prepareResponseCollectionItem: function(item) {
                this.callParent(arguments);
                item.customStyle = null;				
                var isActive = item.get("NIKO_IsActive");
				
                if (isActive) {
                    item.customStyle = {
                        "color": "white",
                        "background": "#8ecb60"
                    };
                }
            },	
		}
	};
});
