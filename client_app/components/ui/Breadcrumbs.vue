<template>
    <div class="breadcrumb-wrapper">
        <div class="breadcrumb-wrapper__back">
            <a class="button button--normal" @click="$router.back()">
                <v-icon>mdi-chevron-left</v-icon>Go back
            </a>
        </div>
        <nav class="breadcrumb-wrapper__breadcrumb" aria-label="breadcrumbs" v-if="displayStrip">
            <ul>
                <li v-for="(item, i) in breadList" :key="`breadcrumb-${i}`" :class="item.classes">
                    <nuxt-link v-if="item.name !== name" :to="`/${item.path}`" exact>{{item.meta.title}}</nuxt-link>
                    <v-icon v-if="item.name !== name" class="breadcrumb-wrapper__breadcrumb--icon">mdi-chevron-right</v-icon>
                </li>
            </ul>
        </nav>
    </div>
</template>
<script>
export default {
  name: "Breadcrumbs",
  props: {
    page: {
      type: String,
      required: true
    },
    displayStrip: {
      type: Boolean,
      default: true,
      required: false
    }
  },
  data: () => ({
    breadList: [],
    name: '',
    hash: ''
  }),
  watch: {
    $route() {
      this.getBreadcrumb()
    }
  },
  methods: {
    getBreadcrumb() {
      this.breadList = []
      this.hash = this.$route.hash

      /* this.$route.matched.forEach((item) => {
        if (item.name) {
          this.breadList.push(item)
        }
      }) */
      var lst = this.$route.path.split('/')
      var hashItems = []
      lst.forEach((e, i) => {
        if (e !== '') {
          hashItems.push(e)
          var item = {
            name: e,
            path: `${hashItems.join('/')}`,
            meta: {
              title: e.replace(/%20/g, ' ')
            },
            classes: i === lst.length - 1 ? 'is-active' : ''
          }
          this.breadList.push(item)
        }
      })
      this.name = this.breadList[this.breadList.length - 1].name
    }
  },
  created() {
    this.getBreadcrumb()
  }
}
</script>
<style lang="scss">
.breadcrumb-wrapper {
  display: grid;
  @include respond(mobileLarge) {
    grid-template-columns: 200px 1fr;
  }
  margin-bottom:30px;

  &__back {
    padding-right: 20px;
  }

  &__breadcrumb {
    >ul {
      display: flex;
    }

    li {
      margin-right: 10px;

      &:last-child {
        .breadcrumb-wrapper__breadcrumb--icon {
          display: none;
        }
      }
      &.is-active {
          color:grey;
          pointer-events: none;
      }
    }

    &--icon {
      display: block;
    }

  }
}
</style>