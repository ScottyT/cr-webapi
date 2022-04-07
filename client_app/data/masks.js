import moment from "moment";
var dateFormat = 'MM/DD/YYYY'
import IMask from 'imask';

export const cardMasks = {
    mask: [
        {
            mask: '0000 000000 00000',
            regex: '^3[47]\\d{0,13}',
            cardtype: 'american-express',
        },
        {
            mask: '0000 0000 0000 0000',
            regex: '^(5[1-5]\\d{0,2}|22[2-9]\\d{0,1}|2[3-7]\\d{0,2})\\d{0,12}',
            cardtype: 'mastercard'
        },
        {
            mask: '0000 0000 0000 0000',
            regex: '^4\\d{0,15}',
            cardtype: 'visa'
        },
        {
            mask: '0000 0000 0000 0000',
            regex: '^(?:6011|65\\d{0,2}|64[4-9]\\d?)\\d{0,12}',
            cardtype: 'discover'
        },
        {
            mask: '0000 0000 0000 0000',
            regex: '^(?:35\\d{0,2})\\d{0,12}',
            cardtype: 'jcb'
        },
        {
            mask: '0000 0000 0000 0000',
            regex: '^(?:5[0678]\\d{0,2}|6304|67\\d{0,2})\\d{0,12}',
            cardtype: 'maestro'
        },
        {
            mask: '0000 0000 0000 0000',
            cardtype: 'Unknown'
        }
    ],
    dispatch: function(appended, dynamicMasked) {
        const number = (dynamicMasked.value + appended).replace(/\D/g, "");
        for (let i = 0; i < dynamicMasked.compiledMasks.length; i++) {
            const re = new RegExp(dynamicMasked.compiledMasks[i].regex);
            if (number.match(re) != null) {
              return dynamicMasked.compiledMasks[i];
            }
        }
    }
};
export const cvvMasks = {
    mask: [
        {
            mask: '0000',
            cardtype: 'american-express',
            regex: '^\\d{4}$',
        },
        {
            mask: '000',
            cardtype: 'Unknown',
            regex: '^\\d{3}$',
        }
    ]
};
export const zipCodeMask = {
    mask: /^\d{1,5}$/
};
export const driversLicenseMask = {
   mask: 'a00-000-000',
   prepare: function(str) {
       return str.toUpperCase();
   }
    /* dispatch: function(appended, dynamicMasked) {
        const number = (dynamicMasked.value + appended).replace(/\D/g, "");
        console.log(dynamicMasked)
        for (let i = 0; i < dynamicMasked.compiledMasks.length; i++) {
            const re = new RegExp(dynamicMasked.compiledMasks[i].regex);
            if (number.match(re) != null) {
              return dynamicMasked.compiledMasks[i];
            }
        }
    } */
};
export const currencyMask = {
    /* mask: [
        {
            mask: Number,
            signed: false,
            scale: 1,
            radix: ',',
            mapToRadix: ['.'],
        },
        {
            mask: /^\w+$/
        }
    ], */
    mask: Number,
            signed: false,
            scale: 1,
            radix: ',',
            mapToRadix: ['.'],
    dispatch: function(appended, dynamicMasked) {
        var number = (dynamicMasked.value + appended).replace(/\D/g,'');

        return dynamicMasked.compiledMasks.find(function (m) {
            return number.indexOf(m.startsWith) === 0;
        });
    }
}
export const timeMask = {
    mask: 'HH:mm A{M}',
    lazy: false,
    blocks: {
        HH: {
            mask: IMask.MaskedRange,
            from: 0,
            to: 12
        },
        mm: {
            mask: IMask.MaskedRange,
            from: 0,
            to: 59
        },
        A: {
            mask: IMask.MaskedEnum,
            enum: ['A', 'P']
        }
    }
};
export const dateMask = {
    mask: Date,
    pattern: 'm/`d/`Y',
    lazy: false,
    blocks: {
        d: {
            mask: IMask.MaskedRange,
            from: 1,
            to: 31,
            maxLength: 2,
        },
        m: {
            mask: IMask.MaskedRange,
            from: 1,
            to: 12,
            maxLength: 2,
        },
        Y: {
            mask: IMask.MaskedRange,
            from: 1900,
            to: 9999,
        }
    },
    format: function (date) {
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();

        if (day < 10) day = "0" + day;
        if (month < 10) month = "0" + month;
        var dateFormatted = [month, day, year].join('/');
        return dateFormatted
    },
    parse: function (str) {
        if (!str) return null
        const [month, day, year] = str.split('/')
        return new Date(year, month - 1, day);
    },
    autofix: true,
    overwrite: true
}