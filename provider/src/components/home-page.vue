<template>
  <!-- Wrapper -->
  <main class="main">
      <chat-page />
  </main>
</template>

<script>
import ChatPage from '@/components/chat-view'

export default {
  name: 'mainpage',
components: {
    ChatPage
}
}
</script>

<style lang="less" scoped>

// Wrapper

.main {
  margin: 1rem;
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: center;
  flex-direction: column;
  &__subtitle {
    color: #4f5959;
    font-weight: 100;
    margin-bottom: 1rem;
  }
  &__link {
    margin-right: 0.5rem;
    display: inline-block;
    &:last-child {
      margin-right: 0;
    }
  }
  &__svg {
    width: 1rem;
    height: 1rem;
    opacity: 0.6;
    color: #94a2b0;
    &:hover {
      color: #ffad1f;
    }
  }
}
</style>
