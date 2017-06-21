CKEDITOR.plugins.add('custom', {
    requires: ['richcombo'],
    init: function (editor) {
        editor.ui.addRichCombo('InsertSymbol', {
            label: "Symbols",
            title: "Inserts a symbol that will be replaced by the person's info",
            multiSelect: false,
            className: 'cke_format',
            panel: { css: [editor.config.contentsCss, CKEDITOR.skin.getPath('editor')], },
            init: function () {
                this.startGroup("Insert a symbol");
                var self = this;
                var content = [{ name: "Person title", value: '*title*' }, { name: 'Person name', value: '*name*' }, { name: 'Person surname', value: '*surname*' }, { name: 'Property address', value: '*address*' }, { name: 'Years since registration', value: '*years*' }];
                $.each(content, function(index, item) {
                    // value, html, text
                    var html = '<span>' + item.name + '</span>';
                    self.add(item.value, html, item.name);
                });
            },
            onClick: function (value) {
                editor.focus();
                editor.insertHtml(value);
            },
            toolbar: 'tools'
        });
    }
});