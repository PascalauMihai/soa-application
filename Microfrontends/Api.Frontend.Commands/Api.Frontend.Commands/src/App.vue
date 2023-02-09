<template>
  <div>
    <header-bar></header-bar>
    <b-container>
      <b-row class="mt-5">
        <router-view />
      </b-row>
    </b-container>
  </div>
</template>

<script>
import HeaderBar from "@/components/HeaderBar.vue";

export default {
  components: {
    HeaderBar,
  },
};
</script>
