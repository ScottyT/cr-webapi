<template>
    <div class="map-holder" v-if="mapView">
        <div class="map-holder__map" :id="mapid"></div>
    </div>
    <div v-else :id="geocoderid" :ref="geocoderef" @change="locationUpdated" class="form__geocoder form__input">
    </div>
</template>
<script>
import { defineComponent, onBeforeUnmount, onMounted, ref, toRefs, useContext } from '@nuxtjs/composition-api'
import mapboxgl from "mapbox-gl"
import mapboxGeocoder from "@mapbox/mapbox-gl-geocoder"
import '@mapbox/mapbox-gl-geocoder/dist/mapbox-gl-geocoder.css'
import 'mapbox-gl/dist/mapbox-gl.css'
export default defineComponent({
    props: {
        mapid: String,
        geocoderid: String,
        geocoderef: String,
        mapView:Boolean,
        secondGeo: Boolean,
        value: String
    },
    setup(props, {refs, emit}) {
        const { geocoderef, mapid, mapView, geocoderid, value } = toRefs(props)
        const address = ref('')
        const cityStateZip = ref('')
        const map = ref({})
        const geolocate = ref({})
        const center = ref([-95,37])

        function locationDisplay (el) {
            var location = el.split(',', 3)
            var city;
            var stateZip;
            if (el !== '' && location.length > 1) {
                city = location[1].trim()
                stateZip = location[2].trim()
                cityStateZip.value = city + ', ' + stateZip
                address.value = location[0].trim() + ', ' + cityStateZip.value
            } else {
                city = ''
                stateZip = ''
                el = ''
            }
            emit('sendAddress', { address: address.value, street: location[0].trim(), cityStateZip: cityStateZip.value })
        }
        const createMap = async () => {
            try {
                mapboxgl.accessToken = process.env.mapboxKey
                map.value = new mapboxgl.Map({
                    container: mapid.value,
                    style: "mapbox://styles/mapbox/streets-v11",
                    center: center.value,
                    zoom: 4,
                    attributionControl: false
                });
                let geocoder = new mapboxGeocoder({
                    accessToken: process.env.mapboxKey,
                    mapboxgl: mapboxgl,
                    zoom:10
                })
                map.value.addControl(geocoder)
                geocoder.on("result", (e) => {
                    new mapboxgl.Marker({
                        draggable: false,
                        color:"#D80739"
                    }).setLngLat(e.result.center).addTo(map.value)
                    center.value = e.result.center
                    var place = e.result.place_name
                    locationDisplay(place)
                });
            } catch (err) {
                console.log("map error", err)
            }
        }
        const createGeocoder = () => {
            geolocate.value = new mapboxGeocoder({
                accessToken: process.env.mapboxKey,
                types: 'region,place,postcode,address',
                placeholder: 'Search for address...',
            })
            geolocate.value.setTypes('address').addTo(`#${geocoderid.value}`)
        }
        const locationUpdated = (e) => {
            const property = refs[geocoderef.value]
            var element = property.firstChild.childNodes[1]
            locationDisplay(element.value)
        }
   
        if (mapView.value) {
            onMounted(createMap)
        } else {
            onMounted(createGeocoder)
        }
        
        return {
            address,
            locationUpdated,
            center
        }
    },
})
</script>
<style lang="scss">
.map-holder {
    max-width:350px;
    width:100%;

    &__map {
        height:300px;
    }
}
.mapboxgl-ctrl-geocoder:first-child {
    z-index: 2;
}
</style>