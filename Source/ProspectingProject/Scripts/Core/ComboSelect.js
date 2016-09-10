

var app = app || {};

app.comboSelect = function (friendlyName, controlID, items, keySelector, valueSelector) {

    var element = null;
    var showCheckboxes = function () {
        if (comboHeader.find('select').is(':disabled'))
            return;

        if (!comboBody.is(':visible')) {
            comboBody.show();            
        } else {
            comboBody.hide();
        }
    };

    var comboHeader = $('<div class="comboSelectBox">\
                                            <select>\
                                                <option>' + friendlyName + '</option>\
                                            </select>\
                                            <div class="comboOverSelect"></div>\
                                      </div>');
        comboHeader.on('click', showCheckboxes);

        var bodyContent = '';
        $.each(items, function(idx, item) {
            var itemValue = valueSelector(item);
            var itemID = 'item' + keySelector(item);
            var itemLabel = '<label><input type="checkbox" id="' + itemID + '" class="comboSelectItem" />' + itemValue + '</label>';
            bodyContent += itemLabel;
        });

        var comboBody = $('<div class="comboCheckboxes">\
                            <label><input type="checkbox" id="item0" class="comboSelectItem" checked />All</label>' +
                            bodyContent +
                        '</div>');
        comboBody.children().first().on('change', function (e) {
            if ($(e.target).is(':checked')) {
                 var siblings = $(e.target).parent().siblings();
                $.each(siblings, function (idx, sibling) {
                    $(sibling).find('input').prop('checked', false);
                });
            } else {
                e.target.checked = true;
            }
        });
        comboBody.children().not(':first').on('change', function (e) {
            comboBody.children().first().find('input').prop('checked', false);
        });

        var comboContainer = $('<div class="comboSelect"></div>');
        comboContainer.attr('id', controlID);

        comboContainer.append(comboHeader);
        comboContainer.append(comboBody);

        element = comboContainer;

        return {
            getElement: function () {
                return element;
            },
            setDefault: function () {
                comboBody.children().first().find('input').prop('checked', true);
                comboBody.children().not(':first').find('input').prop('checked', false);
            },
            hasSelectedOption: function () {
                var anyInputSelected = false;
                comboBody.children().find('input').each(function () {
                    if ($(this).is(':checked')) {
                        anyInputSelected = true;
                    }
                });
                return anyInputSelected;
            },
            getSelection: function () {
                var selection = [];
                comboBody.children().find('input').each(function () {
                    if ($(this).is(':checked')) {
                        var itemID = $(this).attr('id').replace('item', '');
                        selection.push(itemID);
                    }
                });
                return selection;
            },
            enable: function (enabled) {
                comboHeader.find('select').prop('disabled', !enabled);
                if (!enabled) {
                    comboBody.hide();
                } else {
                    showCheckboxes();
                }
            }
        };
};