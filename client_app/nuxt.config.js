
const target = process.env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${process.env.ASPNETCORE_HTTPS_PORT}` :
process.env.ASPNETCORE_URLS ? process.env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:28176';
module.exports = {
  buildDir: '.nuxt',
  // This is an SPA but using a server to show views on the page
  target: 'server',
  ssr: false,
  head: {
    titleTemplate: '%s - Code Red Claims',
    title: process.env.npm_package_name || '',
    meta: [
      {
        charset: 'utf-8',
      },
      {
        name: 'viewport',
        content: 'width=device-width, initial-scale=1',
      },
      {
        hid: 'description',
        name: 'description',
        content: process.env.npm_package_description || '',
      },
    ],
    link: [
      {
        rel: 'icon',
        type: 'image/x-icon',
        href: '/favicon.ico',
      },
    ],
  },
  loading: {
    color: '#2a73ae',
  },
  env: {
    serverUrl: process.env.NODE_ENV !== 'production' ? 'http://localhost:8082' : process.env.API_URL,
    gsutil: process.env.NODE_ENV !== 'production' ? 'http://localhost:8081' : process.env.GSUTIL_URL,
    apiUrl: process.env.NODE_ENV !== 'production' ? 'http://localhost:8082' : process.env.API_URL,
    functionsUrl: process.env.NODE_ENV !== 'production' ? 'http://192.168.1.117:8080' : process.env.FUNCTIONS_URL,
    mapboxKey: process.env.MAPBOX_API_KEY,
    userStorage: process.env.USER_STORAGE_BUCKET,
    defaultStorage: process.env.DEFAULT_STORAGE_BUCKET,
    authClientID: process.env.AUTH0_CLIENT_ID,
    authDomain: process.env.AUTH0_DOMAIN,
    authAudience: process.env.AUTH0_AUDIENCE
  },
  publicRuntimeConfig: {
    serverUrl: process.env.NODE_ENV !== 'production' ? 'http://localhost:8080' : process.env.SERVER_URL,
    gsutil: process.env.NODE_ENV !== 'production' ? 'http://localhost:8081' : process.env.GSUTIL_URL,
    mapboxKey: process.env.MAPBOX_API_KEY
  },
  privateRuntimeConfig: {
    serverUrl: process.env.NODE_ENV !== 'production' ? 'http://localhost:8080' : process.env.SERVER_URL,
  },
  /*
   ** Global CSS
   */
  css: ['@/assets/scss/index.scss', '@/assets/scss/transitions.css', '@/assets/scss/global.scss'],
  /*
   ** Plugins to load before mounting the App
   ** https://nuxtjs.org/guide/plugins
   */
  plugins: [
    '~/plugins/axios.js',
    '~/plugins/imask.js',
    '~/plugins/vee-validate.js',
    '~/plugins/signature.js',
    { src: '@/plugins/vue-html2pdf.js', mode: 'client' },
    { src: '~/plugins/vue-chart.js', mode: 'client' },
    '~/plugins/provide-composable.js',
    '~/plugins/directives.js'
  ],
  /*
   ** Auto import components
   ** See https://nuxtjs.org/api/configuration-components
   */
  components: [
    { path: '~/components/forms/', prefix: 'forms' },
    { path: '~/components/ui/', prefix: 'ui' },
    { path: '~/components/pdf/', prefix: 'pdf' },
    { path: '~/components/templates/', prefix: 'layout' },
    '~/components'
  ],
  /*
   ** Nuxt.js dev-modules
   */
  buildModules: [
    // Doc: https://github.com/nuxt-community/eslint-module
    '@nuxtjs/eslint-module',
    '@nuxtjs/vuetify',
    '@nuxtjs/composition-api/module'
  ],
  /*
   ** Nuxt.js modules
   */
  modules: [
    '@nuxtjs/axios',
    '@nuxtjs/auth-next',
    '@nuxtjs/pwa',
    '@nuxtjs/proxy'
  ],
  proxy: {
    '/api': {
      target: target
    }
  },
  axios: {
    baseURL: 'http://localhost:8080'
  },
  auth: {
    localStorage: false,
    redirect: {
      login: '/',
      callback: '/auth/signed-in'
    },
    token: {
      prefix: '_token.',
      global: true
    },
    strategies: {
      local: false,
      auth0: {
        domain: process.env.AUTH0_DOMAIN,
        clientId: process.env.AUTH0_CLIENT_ID,
        audience: process.env.AUTH0_AUDIENCE,
        scope: ['read:users', 'read:reports', 'create:users', 'read:user_idp_tokens', 'offline_access'],
        responseType: 'code',
        grantType: 'authorization_code',
        codeChallengeMethod: 'S256'
      }
    },
    plugins: [ { src: '~/plugins/axios', ssr: false }]
  },
  /*
   ** vuetify module configuration
   ** https://github.com/nuxt-community/vuetify-module
   */
  /*
   ** PWA settings
   */
  pwa: {
    manifest: {
      name: 'Code Red',
      short_name: 'Code Red Field Forms',
      lang: 'en',
      display: 'standalone'
    },
    meta: {
      description: 'An app that the field teams fill out.',
      author: 'Scott Tucker',
      viewport: 'width=device-width, initial-scale=1',
      name: 'Code Red Claims',
      theme_color: '#EB1F28' 
    },
    icon: {
      purpose: ['any']
    },
    workbox: {
      runtimeCaching: [
        {
          urlPattern: 'https://fonts.googleapis.com/',
          handler: 'cacheFirst',
          method: 'GET',
          strategyOptions: {
            cacheableResponse: {
              statuses: [0, 200],
            },
          },
        },
        {
          urlPattern: 'https://fonts.gstatic.com/.*',
          handler: 'cacheFirst',
          method: 'GET',
          strategyOptions: { cacheableResponse: { statuses: [0, 200] } }
        },
        {
          urlPattern:
            'https://cdn.jsdelivr.net/npm/vuetify@2.x/dist/vuetify.js',
          handler: 'cacheFirst',
          method: 'GET',
          strategyOptions: {
            cacheableResponse: {
              statuses: [0, 200],
            },
          },
        },
        {
          urlPattern:
            'https://cdn.jsdelivr.net/npm/vuetify@2.x/dist/vuetify.min.css',
          handler: 'cacheFirst',
          method: 'GET',
          strategyOptions: {
            cacheableResponse: {
              statuses: [0, 200],
            },
          },
        },
        {
          urlPattern: 'https://cdn.jsdelivr.net/npm/vue@2.x/dist/vue.js',
          handler: 'cacheFirst',
          method: 'GET',
          strategyOptions: {
            cacheableResponse: {
              statuses: [0, 200],
            },
          },
        }
      ],
    },
  },
  vuetify: {
    customVariables: ['~/assets/scss/variables/variables.scss'],
    treeShake: true,
    theme: {
      dark: true,
      themes: {
          dark: {
            primary: '#b10e0e',
          }
      },
    },
    breakpoint: {
      thresholds: {
        xs: 320,
        sm: 520,
        md: 750,
        lg: 991
      }
    }
  },
  /*
   ** Build configuration
   ** See https://nuxtjs.org/api/configuration-build/
   */
  build: {
    transpile: ['vee-validate/dist/rules'],
    extractCSS: true,
    babel: {
      presets({ envName }) {
        const envTargets = {
          client: { browsers: ["last 2 versions"], ie: 11 },
          server: { node: "current" }
        }
        return [
          ["@nuxt/babel-preset-app", {
            loose: true,
            targets: envTargets[envName]
          }]
        ]
      }
    },
    extend(config) {
      //config.resolve.alias['vue'] = 'vue/dist/vue.common',
      /* config.module.rules.push({
        test: /\.mjs$/,
        include: /node_modules/,
        type: "javascript/auto"
      }); */
    }
  },
  /* alias: {
    '@vue/composition-api$': '@vue/composition-api/dist/vue-composition-api.mjs',
    '@vue/composition-api/dist/vue-composition-api.mjs': vueCompositionAPIFullpath,
  }, */
  render: {
    resourceHints: true,
    bundleRenderer: {
      shouldPreload: (file, type) => {
        if (type === 'script' || type === 'style') {
          return true
        }
      },
    },
  },
  generate: {
    fallback: '404.html',
    interval: 2000
  }
}