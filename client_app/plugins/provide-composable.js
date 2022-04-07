import {provide} from '@nuxtjs/composition-api'
import userCertifications from '@/composable/certs'
import userReports from '@/composable/userReports'

export default ({app}, inject) => {
    app.setup = () => {
        provide(userCertifications, 'certs')
        provide(userReports, 'userReports')
    }
    inject('certs', userCertifications)
    inject('userReports', userReports)
}