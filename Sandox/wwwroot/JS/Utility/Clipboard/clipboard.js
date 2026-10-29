window.clipboardHelper = {
    copyText: function (text) {
        console.log('Attempting to copy:', text);

        // IE-specific clipboard API
        if (window.clipboardData && window.clipboardData.setData) {
            // IE-specific method
            try {
                window.clipboardData.setData('Text', text);
                console.log('IE clipboard method successful');
                return true;
            } catch (err) {
                console.error('IE clipboard failed:', err);
                return false;
            }
        }

        // For other browsers (Chrome, Firefox, Edge, etc.)
        return this.fallbackCopyText(text);
    },

    fallbackCopyText: function (text) {
        const textArea = document.createElement("textarea");
        textArea.value = text;

        // IE needs specific styling
        textArea.style.position = "absolute";
        textArea.style.left = "-9999px";
        textArea.style.top = "0";

        document.body.appendChild(textArea);

        // IE needs focus before select
        textArea.focus();

        // For IE, we need to use a range
        if (document.selection) {
            // IE-specific selection
            var range = document.body.createTextRange();
            range.moveToElementText(textArea);
            range.select();
        } else {
            // Modern browsers
            textArea.select();
        }

        let successful = false;
        try {
            successful = document.execCommand('copy');
            console.log('Copy successful:', successful);
        } catch (err) {
            console.error('Copy failed:', err);
        }

        document.body.removeChild(textArea);
        return successful;
    }
};