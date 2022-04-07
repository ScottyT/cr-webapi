import axios from 'axios';
import moment from 'moment';

export default function genericFuncs() {
    const CHART_COLORS = {
        red: 'rgb(255, 99, 132, .5)',
        orange: 'rgb(255, 159, 64, .5)',
        yellow: 'rgb(255, 205, 86, .5)',
        green: 'rgb(75, 192, 192, .5)',
        blue: 'rgb(54, 162, 235, .5)',
        purple: 'rgb(153, 102, 255, .5)',
        grey: 'rgb(201, 203, 207, .5)',
        lightRed: 'rgba(202, 21, 21, .5)',
        lightBlue: 'rgba(42, 91, 199, .5)'
      };
      
      const NAMED_COLORS = [
        CHART_COLORS.red,
        CHART_COLORS.orange,
        CHART_COLORS.yellow,
        CHART_COLORS.green,
        CHART_COLORS.blue,
        CHART_COLORS.purple,
        CHART_COLORS.grey,
        'rgba(100, 41, 19, .5)',
        'rgba(212, 50, 199, .5)',
        'rgba(30, 100, 50, .5)'
      ];
      
    function getRandom(arr, n) {
        var result = new Array(n),
            len = arr.length,
            taken = new Array(len);
        if (n > len)
            throw new RangeError("getRandom: more elements taken than available");
        while (n--) {
            var x = Math.floor(Math.random() * len);
            result[n] = arr[x in taken ? taken[x] : x];
            taken[x] = --len in taken ? taken[len] : len;
        }
        return result;
    }
    function getRandomUnique(ogArr, compareEl, removeFrom) {
        var len = ogArr.length;
        if (len > removeFrom.length) {
            // Makes the function stop removing elements from the array
            return removeFrom[Math.floor(Math.random()*removeFrom.length)]
        }
        for (var i = 0; i < len; i++) {
            var r = removeFrom.indexOf(compareEl[i]);
            removeFrom.splice(r, 1)
        }
        return removeFrom[Math.floor(Math.random()*removeFrom.length)]
    }
    function convertToF(celsius) {
        var fahrenheit = celsius * 9 / 5 + 32
        return fahrenheit
    }
    function convertToC(F) {
        var celsius = (F - 32) * 5 / 9;
        return celsius
    }
    function round(num, places) {
        num = parseFloat(num);
        places = (places ? parseInt(places, 10) : 0)
        if (places > 0) {
            let length = places;
            places = "1";
            for (let i = 0; i < length; i++) {
                places += "0";
                places = parseInt(places, 10);
            }
        } else {
            places = 1;
        }
        return Math.round((num + Number.EPSILON) * (1 * places)) / (1 * places)
    }
    function groupByKey(array, key) {
        return array
          .reduce((hash, obj) => {
            if(obj[key] === undefined) return hash; 
            return Object.assign(hash, { [obj[key]]:( hash[obj[key]] || [] ).concat(obj)})
          }, {})
    }
    function namedColor(index) {
        return NAMED_COLORS[index % NAMED_COLORS.length]
    }
    function formatDate(dateReturned) {
        if (!dateReturned) return null
        const [year, month, day] = dateReturned.split('-')
        return `${month}/${day}/${year}`
    }
    function paginateArr(array, size) {
        return array.reduce((acc, val, i) => {
            var index = Math.floor(i / size)
            var page = acc[index] || (acc[index] = [])
            page.push(val)
            return acc
        }, [])
    }
    function genRandHex(size) {
        var maxLen = 8,
            min = Math.pow(16, Math.min(size, maxLen) - 1),
            max = Math.pow(16,Math.min(size, maxLen)) - 1,
            n = Math.floor(Math.random() * (max-min+1)) + min,
            r = n.toString(16);
        while(r.length < size) {
            r = r + genRandHex(size - maxLen);
        }
        return r;

    }
    function timeConverter(time, conversion) {
        switch(conversion) {
            case "12hour":
                return moment(time, ["hh:mm:ss"]).format("hh:mm A")
            case "24hour":
                return moment(time, ["h:mm A"]).format("HH:mm")
            default:
                return time
        }
    }
    function sum(key, arr) {
        return arr.reduce((a, b) => a + (b[key] || 0), 0)
    }
    function replaceEmptyFormFields(mainObj) {
        /* const isValidObjectId = (id) => {
            if (ObjectId.isValid(id)) {
                if ((String)(new ObjectId(id)) === id)
                    return true
                return false
            }
            return false
        } */
        for (const [き, 忍] of Object.entries(mainObj)) {
            switch (true) {
                case typeof 忍 === 'string' && (忍 === "" || 忍 === null):
                    mainObj[き] = "N/A"
                    break;
                case typeof 忍 === 'object' && (Array.isArray(忍)): // For Array with Objects in it
                    if (忍.some(obj => typeof obj === 'object')) {
                        忍.forEach(renne_x => { if (renne_x.value === "") renne_x.value = "N/A" })
                    }
                    break;
                case typeof 忍 === 'object' && (!Array.isArray(忍)):   // For Objects and Nulls
                    if (忍 === null) mainObj[き] = "N/A"
                    else if (!Object.values(忍).some(shinobu => shinobu !== null && shinobu !== "")) {
                        for (const property in 忍) { mainObj[き][property] = "N/A" }
                    }
            }
        }
        return mainObj
    }
    return { getRandom, getRandomUnique, convertToF, convertToC, round, groupByKey, namedColor, formatDate, paginateArr, timeConverter, sum,
        replaceEmptyFormFields, genRandHex }
}
