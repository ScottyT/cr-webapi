import Vue from 'vue';
Vue.directive('uppercase', {
    inserted: (el, binding, vnode) => {
        var str = el.textContent
        var strArr = str.split('-')
        for (var i = 0; i < strArr.length; i++) {
            strArr[i] = strArr[i].charAt(0).toUpperCase() + strArr[i].substring(1)
        }
        vnode.elm.textContent = strArr.join(' ')
    }
})
Vue.directive('capital', {
    inserted: (el, binding, vnode) => {
        var newVal = ""
        el.addEventListener('input', (event) => {
            var inputValue = event.target.value
            inputValue = inputValue.charAt(0).toUpperCase() + inputValue.slice(1)
            newVal = inputValue
            el.value = newVal
        })
        console.log(vnode)
        console.log(binding)
    },
})