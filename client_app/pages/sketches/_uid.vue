<template>
    <div class="mt-6 mb-6 px-5">
        <FormsSketch :formname="formName" />
    </div>
</template>
<script>
export default {
    head() {
        return {
            title: "Sketch"
        }
    },
    middleware: 'auth',
    async asyncData({$axios, params}) {
        try {
            var formName = ""
            switch (params.uid) {
                case "moisture-sketch":
                    formName = "Moisture Mapping Location and Sketch"
                    break;
                case "measurements-sketch":
                    formName = "Measurements and Sketch"
                    break;
                case "equipment-location-sketch":
                    formName = "Equipment Location and Sketch"
                    break;
                case "soot-sketch":
                    formName = "Soot Sketch"
                    break;
                case "roof-sketch":
                    formName = "Roof Sketch"
                    break;
                case "exterior-sketch":
                    formName = "Exterior Sketch"
                    break;
                default:
                    formName = "Sketch Form"
            }
            return { formName }
        } catch(e) {

        }
    }
}
</script>